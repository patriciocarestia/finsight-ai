using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Rates.Queries.GetCryptoHistory;

public class GetCryptoHistoryQueryHandler
    : IRequestHandler<GetCryptoHistoryQuery, IEnumerable<CryptoRateResponse>>
{
    private readonly IRateRepository rateRepository;

    public GetCryptoHistoryQueryHandler(IRateRepository rateRepository)
    {
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        this.rateRepository = rateRepository;
    }

    public async Task<IEnumerable<CryptoRateResponse>> Handle(
        GetCryptoHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var rates = await this.rateRepository.GetCryptoHistoryAsync(
            request.Symbol,
            request.Days,
            cancellationToken
        );
        return rates.Select(r => CryptoRateResponse.FromEntity(r));
    }
}
