using DolarEnVivo.Application.Interfaces;
using DolarEnVivo.Domain.Entities;
using DolarEnVivo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DolarEnVivo.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly AppDbContext context;

    public PositionRepository(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        this.context = context;
    }

    public async Task<IEnumerable<Position>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken
    ) =>
        await this
            .context.Positions.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Position?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await this.context.Positions.FindAsync([id], cancellationToken);

    public async Task<Position> AddAsync(Position position, CancellationToken cancellationToken)
    {
        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(cancellationToken);
        return position;
    }

    public async Task<Position> UpdateAsync(Position position, CancellationToken cancellationToken)
    {
        this.context.Positions.Update(position);
        await this.context.SaveChangesAsync(cancellationToken);
        return position;
    }

    public async Task DeleteAsync(Position position, CancellationToken cancellationToken)
    {
        this.context.Positions.Remove(position);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
