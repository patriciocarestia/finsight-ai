using DolarEnVivo.Application.DTOs;
using DolarEnVivo.Application.Interfaces;
using DolarEnVivo.Domain.Entities;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Portfolio.Commands.AddPosition;

public class AddPositionCommandHandler : IRequestHandler<AddPositionCommand, PositionResponse>
{
    private readonly IPositionRepository positionRepository;

    public AddPositionCommandHandler(IPositionRepository positionRepository)
    {
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        this.positionRepository = positionRepository;
    }

    public async Task<PositionResponse> Handle(
        AddPositionCommand request,
        CancellationToken cancellationToken
    )
    {
        var position = new Position
        {
            UserId = request.UserId,
            AssetType = request.AssetType,
            Amount = request.Amount,
            PurchasePrice = request.PurchasePrice,
            PurchaseDate = request.PurchaseDate,
            Notes = request.Notes,
            InterestRate = request.InterestRate,
            MaturityDate = request.MaturityDate,
        };

        var created = await this.positionRepository.AddAsync(position, cancellationToken);
        return PositionResponse.FromEntity(created);
    }
}
