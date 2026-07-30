using FinsightAI.Application.Interfaces;
using FinsightAI.Infrastructure.ExternalApis;

namespace FinsightAI.Infrastructure.Services;

public class RatesFetcherService
{
    // The dashboard's longest history range is 90 days; anything older than that
    // is never read, so we keep a small buffer past it and drop the rest.
    private const int RetentionDays = 100;

    private readonly DolarApiClient dolarApiClient;
    private readonly CoinGeckoClient coinGeckoClient;
    private readonly IRateRepository rateRepository;

    public RatesFetcherService(
        DolarApiClient dolarApiClient,
        CoinGeckoClient coinGeckoClient,
        IRateRepository rateRepository
    )
    {
        ArgumentNullException.ThrowIfNull(dolarApiClient, nameof(dolarApiClient));
        ArgumentNullException.ThrowIfNull(coinGeckoClient, nameof(coinGeckoClient));
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        this.dolarApiClient = dolarApiClient;
        this.coinGeckoClient = coinGeckoClient;
        this.rateRepository = rateRepository;
    }

    public async Task FetchAndStoreAllRatesAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var exchangeRates = (await this.dolarApiClient.FetchAllRatesAsync(cts.Token)).ToList();

        if (exchangeRates.Count > 0)
            await this.rateRepository.AddExchangeRatesAsync(exchangeRates, cts.Token);

        var blueRate = exchangeRates.FirstOrDefault(r => r.Type == "blue")?.Sell ?? 1000m;
        var cryptoRates = (
            await this.coinGeckoClient.FetchRatesAsync(blueRate, cts.Token)
        ).ToList();

        if (cryptoRates.Count > 0)
            await this.rateRepository.AddCryptoRatesAsync(cryptoRates, cts.Token);
    }

    public async Task CleanupOldRatesAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);
        await this.rateRepository.DeleteRatesOlderThanAsync(cutoff, cts.Token);
    }
}
