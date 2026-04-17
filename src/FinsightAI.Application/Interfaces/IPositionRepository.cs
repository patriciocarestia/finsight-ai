using FinsightAI.Domain.Entities;

namespace FinsightAI.Application.Interfaces;

public interface IPositionRepository
{
    Task<IEnumerable<Position>> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<Position?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Position> AddAsync(Position position, CancellationToken cancellationToken);
    Task<Position> UpdateAsync(Position position, CancellationToken cancellationToken);
    Task DeleteAsync(Position position, CancellationToken cancellationToken);
}
