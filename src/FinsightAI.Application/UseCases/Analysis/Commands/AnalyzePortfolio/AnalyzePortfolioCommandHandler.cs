using System.Text.Json;
using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Analysis.Commands.AnalyzePortfolio;

public class AnalyzePortfolioCommandHandler : IRequestHandler<AnalyzePortfolioCommand, AnalysisResponse>
{
    private readonly IPositionRepository positionRepository;
    private readonly IRateRepository rateRepository;
    private readonly IGeminiClient geminiClient;

    public AnalyzePortfolioCommandHandler(
        IPositionRepository positionRepository,
        IRateRepository rateRepository,
        IGeminiClient geminiClient)
    {
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        ArgumentNullException.ThrowIfNull(geminiClient, nameof(geminiClient));
        this.positionRepository = positionRepository;
        this.rateRepository = rateRepository;
        this.geminiClient = geminiClient;
    }

    public async Task<AnalysisResponse> Handle(AnalyzePortfolioCommand request, CancellationToken cancellationToken)
    {
        var positions = await this.positionRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var exchangeRates = await this.rateRepository.GetLatestRatesAsync(cancellationToken);
        var cryptoRates = await this.rateRepository.GetLatestCryptoRatesAsync(cancellationToken);

        var portfolioJson = JsonSerializer.Serialize(positions.Select(p => new
        {
            p.AssetType,
            p.Amount,
            p.PurchasePrice,
            p.PurchaseDate,
            p.InterestRate,
            p.MaturityDate
        }));

        var ratesJson = JsonSerializer.Serialize(exchangeRates.Select(r => new
        {
            r.Type,
            r.Buy,
            r.Sell
        }));

        var cryptoJson = JsonSerializer.Serialize(cryptoRates.Select(c => new
        {
            c.Symbol,
            c.PriceUsd,
            c.PriceArs,
            c.ChangePercent24h
        }));

        var prompt = $"""
            Eres un analista financiero especializado en inversiones argentinas.

            Portfolio del usuario:
            {portfolioJson}

            Cotizaciones actuales (ARS):
            {ratesJson}

            Criptomonedas:
            {cryptoJson}

            Inflación estimada últimos 12 meses: ~140%

            Analizá el desempeño del portfolio: qué está rindiendo bien, qué está perdiendo, y compará los retornos contra la inflación y contra mantener dólares blue.
            Sé específico con números y porcentajes.
            Dá 3 sugerencias concretas y accionables para mejorar el portfolio.
            Respondé en español, de forma clara y concisa.
            """;

        var analysis = await this.geminiClient.GenerateContentAsync(prompt, cancellationToken);

        return new AnalysisResponse
        {
            Analysis = analysis,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
