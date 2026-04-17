using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Rates.Queries.GetRateHistory;

public class GetRateHistoryQuery : IRequest<IEnumerable<RateResponse>>
{
    public string Type { get; set; } = string.Empty;
    public int Days { get; set; } = 30;
}
