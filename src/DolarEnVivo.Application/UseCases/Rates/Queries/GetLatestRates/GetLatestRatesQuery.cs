using DolarEnVivo.Application.DTOs;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Rates.Queries.GetLatestRates;

public class GetLatestRatesQuery : IRequest<LatestRatesResponse> { }
