using FinsightAI.API.Controllers.Base;
using FinsightAI.Application.DTOs;
using FinsightAI.Application.UseCases.Portfolio.Commands.AddPosition;
using FinsightAI.Application.UseCases.Portfolio.Commands.DeletePosition;
using FinsightAI.Application.UseCases.Portfolio.Commands.UpdatePosition;
using FinsightAI.Application.UseCases.Portfolio.Queries.GetPositions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinsightAI.API.Controllers;

/// <summary>
/// Manages the authenticated user's investment portfolio
/// </summary>
[Authorize]
public class PortfolioController : BaseController
{
    public PortfolioController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Gets all positions for the authenticated user
    /// </summary>
    [HttpGet]
    [Produces("application/json", Type = typeof(IEnumerable<PositionResponse>))]
    [ProducesResponseType(typeof(IEnumerable<PositionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPositionsAsync(CancellationToken cancellationToken) =>
        Ok(await this.Mediator.Send(new GetPositionsQuery { UserId = this.CurrentUserId }, cancellationToken));

    /// <summary>
    /// Adds a new position to the portfolio
    /// </summary>
    [HttpPost]
    [Produces("application/json", Type = typeof(PositionResponse))]
    [ProducesResponseType(typeof(PositionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPositionAsync([FromBody] AddPositionCommand command, CancellationToken cancellationToken)
    {
        command.UserId = this.CurrentUserId;
        var result = await this.Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPositionsAsync), result);
    }

    /// <summary>
    /// Updates an existing position
    /// </summary>
    [HttpPut("{id:int}")]
    [Produces("application/json", Type = typeof(PositionResponse))]
    [ProducesResponseType(typeof(PositionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePositionAsync(int id, [FromBody] UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        command.UserId = this.CurrentUserId;
        return Ok(await this.Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Deletes a position from the portfolio
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePositionAsync(int id, CancellationToken cancellationToken)
    {
        await this.Mediator.Send(new DeletePositionCommand { Id = id, UserId = this.CurrentUserId }, cancellationToken);
        return NoContent();
    }
}
