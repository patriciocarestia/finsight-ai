using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Portfolio.Queries.GetPositions;

public class GetPositionsQuery : IRequest<IEnumerable<PositionResponse>>
{
    public int UserId { get; set; }
}
