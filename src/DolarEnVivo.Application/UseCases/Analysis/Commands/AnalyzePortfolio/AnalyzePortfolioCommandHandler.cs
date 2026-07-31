using System.Text.Json;
using DolarEnVivo.Application.DTOs;
using DolarEnVivo.Application.Interfaces;
using MediatR;

namespace DolarEnVivo.Application.UseCases.Analysis.Commands.AnalyzePortfolio;

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

        var ratesJson = JsonSerializer.Serialize(exchangeRates.Select(r => new { r.Type, r.Sell }));

        var cryptoJson = JsonSerializer.Serialize(
            cryptoRates.Select(c => new { c.Symbol, c.PriceArs })
        );

        var today = DateTime.UtcNow.Date;

        var portfolioData = positions.Select(p => new
        {
            p.AssetType,
            p.Amount,
            p.PurchasePrice,
            p.PurchaseDate,
            DaysSincePurchase = (today - p.PurchaseDate.Date).Days,
        });

        var portfolioJson = JsonSerializer.Serialize(portfolioData);

        var prompt = $"""
            Sos un asesor financiero argentino.

            Contexto:
            - Fecha actual (UTC): {today:yyyy-MM-dd}
            - Cada posición incluye "DaysSincePurchase" (días desde la compra)

            Reglas de análisis:
            - Separar SIEMPRE rendimiento nominal vs real
            - Si DaysSincePurchase < 30:
            → NO evaluar contra inflación
            → NO asumir variaciones de mercado
            → Solo mostrar estado nominal actual
            - Solo evaluar rendimiento real si pasaron suficientes días (>= 30)
            - No inventar cambios de precio si no están explícitos en los datos

            Reglas de respuesta:
            - Máx 300 palabras
            - Tono directo, sin relleno
            - Usar números concretos (ARS y %)
            - No explicar conceptos básicos

            Formato:

            ## Resumen
            1 frase clara del estado general

            ## Performance
            Por cada posición:
            - Inversión vs valor actual
            - Ganancia/pérdida nominal (%)
            - Insight corto

            ## Real vs inflación
            - Solo si DaysSincePurchase >= 30
            - Si no: aclarar que no aplica aún

            ## Recomendaciones
            3 acciones concretas

            Datos:

            Portfolio:
            {portfolioJson}

            Dólar (ARS):
            {ratesJson}

            Crypto:
            {cryptoJson}

            Inflación anual estimada: 140%

            Objetivo:
            Evaluar correctamente según el tiempo real de cada inversión.
            No asumir escenarios históricos si la inversión es reciente.
            """;

        var analysis = await geminiClient.GenerateContentAsync(prompt, cancellationToken);

        return new AnalysisResponse { Analysis = analysis, GeneratedAt = DateTime.UtcNow };
    }
}
