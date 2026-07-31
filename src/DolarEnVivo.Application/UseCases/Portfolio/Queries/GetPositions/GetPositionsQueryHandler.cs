using DolarEnVivo.Application.DTOs;
using DolarEnVivo.Application.Interfaces;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Portfolio.Queries.GetPositions;

public class GetPositionsQueryHandler
    : IRequestHandler<GetPositionsQuery, IEnumerable<PositionResponse>>
{
    private readonly IPositionRepository positionRepository;

    public GetPositionsQueryHandler(IPositionRepository positionRepository)
    {
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        this.positionRepository = positionRepository;
    }

    public async Task<IEnumerable<PositionResponse>> Handle(
        GetPositionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var positions = await this.positionRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken
        );
        return positions.Select(PositionResponse.FromEntity);
    }
}
