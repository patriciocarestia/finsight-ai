using System.Text.Json;
using System.Text.Json.Serialization;
using FinsightAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FinsightAI.Infrastructure.ExternalApis;

public class CoinGeckoClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<CoinGeckoClient> logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CoinGeckoClient(HttpClient httpClient, ILogger<CoinGeckoClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<IEnumerable<CryptoRate>> FetchRatesAsync(decimal blueRate, CancellationToken cancellationToken)
    {
        try
        {
            var url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd&include_24hr_change=true";
            var response = await this.httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<Dictionary<string, CoinPriceDto>>(json, JsonOptions) ?? [];

            var rates = new List<CryptoRate>();

            if (data.TryGetValue("bitcoin", out var btc))
            {
                rates.Add(new CryptoRate
                {
                    Symbol = "BTC",
                    PriceUsd = btc.Usd,
                    PriceArs = btc.Usd * blueRate,
                    ChangePercent24h = btc.UsdChange24h,
                    RecordedAt = DateTime.UtcNow
                });
            }

            if (data.TryGetValue("ethereum", out var eth))
            {
                rates.Add(new CryptoRate
                {
                    Symbol = "ETH",
                    PriceUsd = eth.Usd,
                    PriceArs = eth.Usd * blueRate,
                    ChangePercent24h = eth.UsdChange24h,
                    RecordedAt = DateTime.UtcNow
                });
            }

            return rates;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to fetch crypto rates from CoinGecko");
            return [];
        }
    }

    public async Task<IEnumerable<CryptoRate>> FetchMarketChartAsync(string coinId, string symbol, IReadOnlyDictionary<DateTime, decimal> blueRateByDate, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=usd&days=90";
            var response = await this.httpClient.GetAsync(url, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("CoinGecko error for {CoinId}. Status: {Status}, Body: {Body}",
                    coinId, response.StatusCode, json[..Math.Min(300, json.Length)]);
                return [];
            }

            var data = JsonSerializer.Deserialize<MarketChartDto>(json, JsonOptions);

            if (data?.Prices is null)
                return [];

            var fallbackBlue = blueRateByDate.Values.DefaultIfEmpty(1200m).Last();

            return data.Prices.Select(p =>
            {
                var date = DateTimeOffset.FromUnixTimeMilliseconds((long)p[0]).UtcDateTime;
                blueRateByDate.TryGetValue(date.Date, out var blueRate);
                if (blueRate == 0) blueRate = fallbackBlue;

                return new CryptoRate
                {
                    Symbol = symbol,
                    PriceUsd = Math.Round(p[1], 2),
                    PriceArs = Math.Round(p[1] * blueRate, 2),
                    ChangePercent24h = 0,
                    RecordedAt = date
                };
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to fetch market chart for {CoinId}", coinId);
            return [];
        }
    }

    private record CoinPriceDto
    {
        [JsonPropertyName("usd")]
        public decimal Usd { get; init; }

        [JsonPropertyName("usd_24h_change")]
        public decimal UsdChange24h { get; init; }
    }

    private record MarketChartDto
    {
        [JsonPropertyName("prices")]
        public List<List<decimal>>? Prices { get; init; }
    }
}
