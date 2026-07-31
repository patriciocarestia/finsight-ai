using System.ComponentModel.DataAnnotations;
using DolarEnVivo.Application.DTOs;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
