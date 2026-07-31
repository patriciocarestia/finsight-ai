using DolarEnVivo.Application.DTOs;
using DolarEnVivo.Application.Interfaces;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Rates.Queries.GetRateHistory;

public class GetRateHistoryQueryHandler
    : IRequestHandler<GetRateHistoryQuery, IEnumerable<RateResponse>>
{
    private readonly IRateRepository rateRepository;

    public GetRateHistoryQueryHandler(IRateRepository rateRepository)
    {
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        this.rateRepository = rateRepository;
    }

    public async Task<IEnumerable<RateResponse>> Handle(
        GetRateHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var rates = await this.rateRepository.GetRateHistoryAsync(
            request.Type,
            request.Days,
            cancellationToken
        );
        return rates.Select(r => RateResponse.FromEntity(r));
    }
}
