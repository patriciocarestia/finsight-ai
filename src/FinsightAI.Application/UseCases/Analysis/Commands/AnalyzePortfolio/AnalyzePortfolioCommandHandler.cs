using System.Text.Json;
using FinsightAI.Application.DTOs;
using FinsightAI.Application.Interfaces;
using MediatR;

namespace FinsightAI.Application.UseCases.Analysis.Commands.AnalyzePortfolio;

public class AnalyzePortfolioCommandHandler
    : IRequestHandler<AnalyzePortfolioCommand, AnalysisResponse>
{
    private readonly IPositionRepository positionRepository;
    private readonly IRateRepository rateRepository;
    private readonly IGeminiClient geminiClient;

    public AnalyzePortfolioCommandHandler(
        IPositionRepository positionRepository,
        IRateRepository rateRepository,
        IGeminiClient geminiClient
    )
    {
        ArgumentNullException.ThrowIfNull(positionRepository);
        ArgumentNullException.ThrowIfNull(rateRepository);
        ArgumentNullException.ThrowIfNull(geminiClient);

        this.positionRepository = positionRepository;
        this.rateRepository = rateRepository;
        this.geminiClient = geminiClient;
    }

    public async Task<AnalysisResponse> Handle(
        AnalyzePortfolioCommand request,
        CancellationToken cancellationToken
    )
    {
        var positions = await positionRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken
        );

        var exchangeRates = await rateRepository.GetLatestRatesAsync(cancellationToken);
        var cryptoRates = await rateRepository.GetLatestCryptoRatesAsync(cancellationToken);

        var portfolioJson = JsonSerializer.Serialize(
            positions.Select(p => new
            {
                p.AssetType,
                p.Amount,
                p.PurchasePrice,
            })
        );

        var ratesJson = JsonSerializer.Serialize(exchangeRates.Select(r => new { r.Type, r.Sell }));

        var cryptoJson = JsonSerializer.Serialize(
            cryptoRates.Select(c => new { c.Symbol, c.PriceArs })
        );

        var prompt = $"""
            Sos un asesor financiero argentino (abril 2026).

            Analizá el portfolio de forma directa, clara y útil.

            Reglas:
            - Máx 300 palabras
            - Sin introducciones ni disclaimers
            - Usá números concretos (ARS y %)
            - Sé preciso y sintético

            Formato:

            ## Resumen
            1 frase clara del estado general.

            ## Performance
            Para cada activo:
            - inversión vs valor actual
            - ganancia/pérdida (ARS y %)
            - insight corto

            ## Real vs inflación
            Compará contra inflación (~140%) y dólar blue.

            ## Recomendaciones
            3 acciones concretas y directas.

            Datos:

            Portfolio:
            {portfolioJson}

            Dólar:
            {ratesJson}

            Crypto:
            {cryptoJson}
            """;

        string analysis;

        try
        {
            analysis = await geminiClient.GenerateContentAsync(prompt, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            analysis = "No se pudo generar el análisis en este momento.";
        }

        return new AnalysisResponse { Analysis = analysis, GeneratedAt = DateTime.UtcNow };
    }
}
