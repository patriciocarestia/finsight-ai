using FinsightAI.API.Controllers.Base;
using FinsightAI.Application.DTOs;
using FinsightAI.Application.UseCases.Auth.Commands.Login;
using FinsightAI.Application.UseCases.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinsightAI.API.Controllers;

/// <summary>
/// Handles user authentication
/// </summary>
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Registers a new user
    /// </summary>
    [HttpPost("register")]
    [Produces("application/json", Type = typeof(AuthResponse))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand command, CancellationToken cancellationToken) =>
        Created(string.Empty, await this.Mediator.Send(command, cancellationToken));

    /// <summary>
    /// Logs in and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    [Produces("application/json", Type = typeof(AuthResponse))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken) =>
        Ok(await this.Mediator.Send(command, cancellationToken));
}
