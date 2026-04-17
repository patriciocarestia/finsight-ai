using FinsightAI.Infrastructure.Data;
using FinsightAI.Infrastructure.ExternalApis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinsightAI.Infrastructure.Services;

public class HistoricalDataSeeder
{
    private readonly AppDbContext context;
    private readonly DolarApiClient dolarApiClient;
    private readonly CoinGeckoClient coinGeckoClient;
    private readonly ILogger<HistoricalDataSeeder> logger;

    private static readonly (string ApiPath, string DbType)[] DollarTypes =
    [
        ("oficial", "oficial"),
        ("blue", "blue"),
        ("bolsa", "mep"),
        ("contadoconliqui", "ccl"),
        ("cripto", "cripto"),
    ];

    public HistoricalDataSeeder(
        AppDbContext context,
        DolarApiClient dolarApiClient,
        CoinGeckoClient coinGeckoClient,
        ILogger<HistoricalDataSeeder> logger)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(dolarApiClient, nameof(dolarApiClient));
        ArgumentNullException.ThrowIfNull(coinGeckoClient, nameof(coinGeckoClient));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.context = context;
        this.dolarApiClient = dolarApiClient;
        this.coinGeckoClient = coinGeckoClient;
        this.logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var hasHistoricalData = await this.context.ExchangeRates
            .AnyAsync(r => r.RecordedAt < yesterday, cancellationToken);
        if (hasHistoricalData)
        {
            this.logger.LogInformation("Historical data already exists, skipping seed.");
            return;
        }

        this.logger.LogInformation("Fetching real historical data from ArgentinaDatos and CoinGecko...");

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMinutes(2));

        var exchangeRates = new List<Domain.Entities.ExchangeRate>();

        foreach (var (apiPath, dbType) in DollarTypes)
        {
            var rates = (await this.dolarApiClient.FetchHistoricalRatesAsync(apiPath, dbType, 90, cts.Token)).ToList();
            this.logger.LogInformation("Fetched {Count} records for {Type}", rates.Count, dbType);
            exchangeRates.AddRange(rates);
        }

        if (exchangeRates.Count > 0)
        {
            this.context.ExchangeRates.AddRange(exchangeRates);
            await this.context.SaveChangesAsync(cts.Token);
            this.logger.LogInformation("Saved {Count} exchange rate records", exchangeRates.Count);
        }

        var blueRateByDate = exchangeRates
            .Where(r => r.Type == "blue" && r.Sell > 0)
            .GroupBy(r => r.RecordedAt.Date)
            .ToDictionary(g => g.Key, g => g.First().Sell);

        var btcHistory = (await this.coinGeckoClient.FetchMarketChartAsync("bitcoin", "BTC", blueRateByDate, cts.Token)).ToList();
        this.logger.LogInformation("Fetched {Count} BTC records from CoinGecko", btcHistory.Count);

        await Task.Delay(TimeSpan.FromSeconds(3), cts.Token);

        var ethHistory = (await this.coinGeckoClient.FetchMarketChartAsync("ethereum", "ETH", blueRateByDate, cts.Token)).ToList();
        this.logger.LogInformation("Fetched {Count} ETH records from CoinGecko", ethHistory.Count);

        var cryptoRates = btcHistory.Concat(ethHistory).ToList();

        if (cryptoRates.Count == 0)
        {
            this.logger.LogWarning("CoinGecko unavailable, generating simulated crypto data anchored to real ARS rates.");
            cryptoRates = GenerateSimulatedCrypto(blueRateByDate);
        }

        if (cryptoRates.Count > 0)
        {
            this.context.CryptoRates.AddRange(cryptoRates);
            await this.context.SaveChangesAsync(cts.Token);
            this.logger.LogInformation("Saved {Count} crypto rate records", cryptoRates.Count);
        }
    }

    private static List<Domain.Entities.CryptoRate> GenerateSimulatedCrypto(IReadOnlyDictionary<DateTime, decimal> blueRateByDate)
    {
        var random = new Random(99);
        var rates = new List<Domain.Entities.CryptoRate>();
        decimal btcUsd = 82_000m;
        decimal ethUsd = 2_700m;
        var fallback = blueRateByDate.Values.DefaultIfEmpty(1200m).Last();

        foreach (var (date, blueRate) in blueRateByDate.OrderBy(x => x.Key))
        {
            btcUsd += btcUsd * (decimal)(random.NextDouble() * 0.04 - 0.018);
            ethUsd += ethUsd * (decimal)(random.NextDouble() * 0.04 - 0.018);
            btcUsd = Math.Max(btcUsd, 50_000m);
            ethUsd = Math.Max(ethUsd, 1_500m);

            var arsRate = blueRate > 0 ? blueRate : fallback;
            var ts = DateTime.SpecifyKind(date.AddHours(12), DateTimeKind.Utc);

            rates.Add(new Domain.Entities.CryptoRate { Symbol = "BTC", PriceUsd = Math.Round(btcUsd, 2), PriceArs = Math.Round(btcUsd * arsRate, 2), ChangePercent24h = 0, RecordedAt = ts });
            rates.Add(new Domain.Entities.CryptoRate { Symbol = "ETH", PriceUsd = Math.Round(ethUsd, 2), PriceArs = Math.Round(ethUsd * arsRate, 2), ChangePercent24h = 0, RecordedAt = ts });
        }

        return rates;
    }
}
