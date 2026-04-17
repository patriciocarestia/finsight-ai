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

    public async Task<LatestRatesResponse> Handle(GetLatestRatesQuery request, CancellationToken cancellationToken)
    {
        var exchangeRates = await this.rateRepository.GetLatestRatesAsync(cancellationToken);
        var cryptoRates = await this.rateRepository.GetLatestCryptoRatesAsync(cancellationToken);

        return new LatestRatesResponse
        {
            ExchangeRates = exchangeRates.Select(RateResponse.FromEntity),
            CryptoRates = cryptoRates.Select(CryptoRateResponse.FromEntity),
            FetchedAt = DateTime.UtcNow
        };
    }
}
