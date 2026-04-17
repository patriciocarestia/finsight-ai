using FinsightAI.Domain.Entities;

namespace FinsightAI.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
