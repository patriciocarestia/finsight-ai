using DolarEnVivo.Application.DTOs;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Portfolio.Queries.GetPositions;

public class GetPositionsQuery : IRequest<IEnumerable<PositionResponse>>
{
    public int UserId { get; set; }
}
