using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Portfolio.Commands.UpdatePosition;

public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, PositionResponse>
{
    private readonly IPositionRepository positionRepository;

    public UpdatePositionCommandHandler(IPositionRepository positionRepository)
    {
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        this.positionRepository = positionRepository;
    }

    public async Task<PositionResponse> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await this.positionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position {request.Id} not found.");

        if (position.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not own this position.");

        position.AssetType = request.AssetType;
        position.Amount = request.Amount;
        position.PurchasePrice = request.PurchasePrice;
        position.PurchaseDate = request.PurchaseDate;
        position.Notes = request.Notes;
        position.InterestRate = request.InterestRate;
        position.MaturityDate = request.MaturityDate;
        position.UpdatedAt = DateTime.UtcNow;

        var updated = await this.positionRepository.UpdateAsync(position, cancellationToken);
        return PositionResponse.FromEntity(updated);
    }
}
