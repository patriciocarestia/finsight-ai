using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Analysis.Commands.AnalyzePortfolio;

public class AnalyzePortfolioCommand : IRequest<AnalysisResponse>
{
    public int UserId { get; set; }
}
