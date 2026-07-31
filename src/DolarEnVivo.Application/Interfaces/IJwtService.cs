using DolarEnVivo.Domain.Entities;

namespace DolarEnVivo.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
