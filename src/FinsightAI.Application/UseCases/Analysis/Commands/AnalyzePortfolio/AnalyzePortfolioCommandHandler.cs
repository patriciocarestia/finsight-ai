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
            Sos un asesor financiero argentino. La fecha actual es abril 2026. Todas las fechas del portfolio son correctas, estamos en 2026. Tu trabajo es analizar el portfolio del usuario y darle feedback útil.
            Reglas de formato y tono:
            - Máximo 600 palabras
            - Tuteá al usuario, hablale de vos como un asesor cercano pero profesional
            - No expliques conceptos básicos, asumí que el usuario entiende de finanzas
            - No pongas disclaimers, introducciones formales ni despedidas
            - Usá números concretos siempre, no redondees de más
            - No uses fórmulas matemáticas, mostrá directamente los resultados
            Estructura fija (respetala siempre):
            ## Resumen rápido
            Una oración con el veredicto general del portfolio.
            ## Posición por posición
            Cuánto invertiste, cuánto vale hoy, ganancia/pérdida en pesos y porcentaje.
            ## Versus inflación
            Cuánto perdió o ganó en términos reales.
            ## Qué haría yo
            2-3 sugerencias concretas y cortas, sin explicar por qué funcionan los instrumentos.

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
