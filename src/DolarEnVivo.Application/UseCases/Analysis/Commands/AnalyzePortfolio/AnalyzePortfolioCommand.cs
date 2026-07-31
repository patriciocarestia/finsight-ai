using DolarEnVivo.Application.DTOs;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Analysis.Commands.AnalyzePortfolio;

public class AnalyzePortfolioCommand : IRequest<AnalysisResponse>
{
    public int UserId { get; set; }
}
