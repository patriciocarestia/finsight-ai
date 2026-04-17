using MediatR;

namespace FinsightAI.Application.UseCases.Portfolio.Commands.DeletePosition;

public class DeletePositionCommand : IRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
}
