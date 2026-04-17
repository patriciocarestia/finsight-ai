using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Portfolio.Commands.DeletePosition;

public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand>
{
    private readonly IPositionRepository positionRepository;

    public DeletePositionCommandHandler(IPositionRepository positionRepository)
    {
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        this.positionRepository = positionRepository;
    }

    public async Task Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await this.positionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position {request.Id} not found.");

        if (position.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not own this position.");

        await this.positionRepository.DeleteAsync(position, cancellationToken);
    }
}
