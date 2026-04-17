using FinsightAI.Application.Interfaces;
using FinsightAI.Infrastructure.ExternalApis;
using Microsoft.Extensions.Logging;

namespace FinsightAI.Infrastructure.Services;

public class RatesFetcherService
{
    private readonly DolarApiClient dolarApiClient;
    private readonly CoinGeckoClient coinGeckoClient;
    private readonly IRateRepository rateRepository;
    private readonly ILogger<RatesFetcherService> logger;

    public RatesFetcherService(
        DolarApiClient dolarApiClient,
        CoinGeckoClient coinGeckoClient,
        IRateRepository rateRepository,
        ILogger<RatesFetcherService> logger)
    {
        ArgumentNullException.ThrowIfNull(dolarApiClient, nameof(dolarApiClient));
        ArgumentNullException.ThrowIfNull(coinGeckoClient, nameof(coinGeckoClient));
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.dolarApiClient = dolarApiClient;
        this.coinGeckoClient = coinGeckoClient;
        this.rateRepository = rateRepository;
        this.logger = logger;
    }

    public async Task FetchAndStoreAllRatesAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        this.logger.LogInformation("Starting rates fetch job at {Time}", DateTime.UtcNow);

        var exchangeRates = (await this.dolarApiClient.FetchAllRatesAsync(cts.Token)).ToList();

        if (exchangeRates.Count > 0)
        {
            await this.rateRepository.AddExchangeRatesAsync(exchangeRates, cts.Token);
            this.logger.LogInformation("Stored {Count} exchange rates", exchangeRates.Count);
        }

        var blueRate = exchangeRates.FirstOrDefault(r => r.Type == "blue")?.Sell ?? 1000m;
        var cryptoRates = (await this.coinGeckoClient.FetchRatesAsync(blueRate, cts.Token)).ToList();

        if (cryptoRates.Count > 0)
        {
            await this.rateRepository.AddCryptoRatesAsync(cryptoRates, cts.Token);
            this.logger.LogInformation("Stored {Count} crypto rates", cryptoRates.Count);
        }
    }
}
