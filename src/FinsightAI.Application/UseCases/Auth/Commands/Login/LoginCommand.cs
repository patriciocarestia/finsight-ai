using System.ComponentModel.DataAnnotations;
using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
