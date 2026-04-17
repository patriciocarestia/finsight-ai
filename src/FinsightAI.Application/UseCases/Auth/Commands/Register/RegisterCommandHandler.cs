using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using FinsightAI.Domain.Entities;
using MediatR;

namespace FinsightAI.Application.UseCases.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository userRepository;
    private readonly IJwtService jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        ArgumentNullException.ThrowIfNull(userRepository, nameof(userRepository));
        ArgumentNullException.ThrowIfNull(jwtService, nameof(jwtService));
        this.userRepository = userRepository;
        this.jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await this.userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        var created = await this.userRepository.AddAsync(user, cancellationToken);
        var token = this.jwtService.GenerateToken(created);

        return new AuthResponse
        {
            Token = token,
            Email = created.Email,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }
}
