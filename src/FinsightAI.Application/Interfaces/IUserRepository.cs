using FinsightAI.Domain.Entities;

namespace FinsightAI.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> AddAsync(User user, CancellationToken cancellationToken);
}
