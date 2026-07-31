using DolarEnVivo.Application.Interfaces;
using DolarEnVivo.Domain.Entities;
using DolarEnVivo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DolarEnVivo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;

    public UserRepository(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        this.context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await this.context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(cancellationToken);
        return user;
    }
}
