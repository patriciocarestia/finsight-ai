using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Rates.Queries.GetCryptoHistory;

public class GetCryptoHistoryQuery : IRequest<IEnumerable<CryptoRateResponse>>
{
    public string Symbol { get; set; } = string.Empty;
    public int Days { get; set; } = 30;
}
