using DolarEnVivo.API.Controllers.Base;
using DolarEnVivo.Application.DTOs;
using DolarEnVivo.Application.UseCases.Analysis.Commands.AnalyzePortfolio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DolarEnVivo.API.Controllers;

/// <summary>
/// Provides AI-powered portfolio analysis via Gemini
/// </summary>
[Authorize]
public class AnalysisController : BaseController
{
    public AnalysisController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Analyzes the user's portfolio using Gemini AI
    /// </summary>
    [HttpPost]
    [Produces("application/json", Type = typeof(AnalysisResponse))]
    [ProducesResponseType(typeof(AnalysisResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AnalyzeAsync(CancellationToken cancellationToken) =>
        Ok(
            await this.Mediator.Send(
                new AnalyzePortfolioCommand { UserId = this.CurrentUserId },
                cancellationToken
            )
        );
}
