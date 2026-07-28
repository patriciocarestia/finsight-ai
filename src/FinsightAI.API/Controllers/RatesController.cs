using FinsightAI.API.Controllers.Base;
using FinsightAI.Application.DTOs;
using FinsightAI.Application.UseCases.Rates.Queries.GetCryptoHistory;
using FinsightAI.Application.UseCases.Rates.Queries.GetLatestRates;
using FinsightAI.Application.UseCases.Rates.Queries.GetRateHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinsightAI.API.Controllers;

/// <summary>
/// Provides exchange rate and crypto price data
/// </summary>
public class RatesController : BaseController
{
    public RatesController(IMediator mediator)
        : base(mediator) { }

    /// <summary>
    /// Gets the latest exchange rates and crypto prices
    /// </summary>
    [HttpGet("latest")]
    [Produces("application/json", Type = typeof(LatestRatesResponse))]
    [ProducesResponseType(typeof(LatestRatesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestAsync(CancellationToken cancellationToken) =>
        Ok(await this.Mediator.Send(new GetLatestRatesQuery(), cancellationToken));

    /// <summary>
    /// Gets historical exchange rate data
    /// </summary>
    [HttpGet("history")]
    [Produces("application/json", Type = typeof(IEnumerable<RateResponse>))]
    [ProducesResponseType(typeof(IEnumerable<RateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryAsync(
        [FromQuery] string type = "blue",
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await this.Mediator.Send(
                new GetRateHistoryQuery { Type = type, Days = days },
                cancellationToken
            )
        );

    /// <summary>
    /// Gets historical crypto price data
    /// </summary>
    [HttpGet("crypto-history")]
    [Produces("application/json", Type = typeof(IEnumerable<CryptoRateResponse>))]
    [ProducesResponseType(typeof(IEnumerable<CryptoRateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCryptoHistoryAsync(
        [FromQuery] string symbol = "BTC",
        [FromQuery] int days = 7,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await this.Mediator.Send(
                new GetCryptoHistoryQuery { Symbol = symbol, Days = days },
                cancellationToken
            )
        );
}
