using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Rates.Queries.GetLatestRates;

public class GetLatestRatesQueryHandler : IRequestHandler<GetLatestRatesQuery, LatestRatesResponse>
{
    private readonly IRateRepository rateRepository;

    public GetLatestRatesQueryHandler(IRateRepository rateRepository)
    {
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        this.rateRepository = rateRepository;
    }

    public async Task<LatestRatesResponse> Handle(
        GetLatestRatesQuery request,
        CancellationToken cancellationToken
    )
    {
        var exchangeRates = await this.rateRepository.GetLatestRatesAsync(cancellationToken);
        var previousExchangeRates = await this.rateRepository.GetPreviousDayRatesAsync(
            cancellationToken
        );
        var cryptoRates = await this.rateRepository.GetLatestCryptoRatesAsync(cancellationToken);
        var previousCryptoRates = await this.rateRepository.GetPreviousDayCryptoRatesAsync(
            cancellationToken
        );

        var prevExchangeByType = previousExchangeRates.ToDictionary(r => r.Type);
        var prevCryptoBySymbol = previousCryptoRates.ToDictionary(r => r.Symbol);

        return new LatestRatesResponse
        {
            ExchangeRates = exchangeRates.Select(r =>
            {
                decimal change = 0;
                if (prevExchangeByType.TryGetValue(r.Type, out var prev) && prev.Sell > 0)
                    change = Math.Round((r.Sell - prev.Sell) / prev.Sell * 100, 2);
                return RateResponse.FromEntity(r, change);
            }),
            CryptoRates = cryptoRates.Select(r =>
            {
                decimal change = 0;
                if (prevCryptoBySymbol.TryGetValue(r.Symbol, out var prev) && prev.PriceArs > 0)
                    change = Math.Round((r.PriceArs - prev.PriceArs) / prev.PriceArs * 100, 2);
                return CryptoRateResponse.FromEntity(r, change);
            }),
            FetchedAt = DateTime.UtcNow,
        };
    }
}
