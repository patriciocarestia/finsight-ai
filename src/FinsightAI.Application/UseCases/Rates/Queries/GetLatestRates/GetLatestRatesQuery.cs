using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Rates.Queries.GetLatestRates;

public class GetLatestRatesQuery : IRequest<LatestRatesResponse>
{
}
