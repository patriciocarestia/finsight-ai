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
        ArgumentNullException.ThrowIfNull(positionRepository, nameof(positionRepository));
        ArgumentNullException.ThrowIfNull(rateRepository, nameof(rateRepository));
        ArgumentNullException.ThrowIfNull(geminiClient, nameof(geminiClient));
        this.positionRepository = positionRepository;
        this.rateRepository = rateRepository;
        this.geminiClient = geminiClient;
    }

    public async Task<AnalysisResponse> Handle(
        AnalyzePortfolioCommand request,
        CancellationToken cancellationToken
    )
    {
        var positions = await this.positionRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken
        );
        var exchangeRates = await this.rateRepository.GetLatestRatesAsync(cancellationToken);
        var cryptoRates = await this.rateRepository.GetLatestCryptoRatesAsync(cancellationToken);

        var portfolioJson = JsonSerializer.Serialize(
            positions.Select(p => new
            {
                p.AssetType,
                p.Amount,
                p.PurchasePrice,
                p.PurchaseDate,
                p.InterestRate,
                p.MaturityDate,
            })
        );

        var ratesJson = JsonSerializer.Serialize(
            exchangeRates.Select(r => new
            {
                r.Type,
                r.Buy,
                r.Sell,
            })
        );

        var cryptoJson = JsonSerializer.Serialize(
            cryptoRates.Select(c => new
            {
                c.Symbol,
                c.PriceUsd,
                c.PriceArs,
                c.ChangePercent24h,
            })
        );

        var prompt = $"""
            Sos un asesor financiero argentino en abril 2026. Analizá el portfolio del usuario con criterio profesional y directo.

            Reglas:
            - Máx. 400 palabras
            - Tono cercano, sin formalidades ni disclaimers
            - Usá números concretos (ARS y %)
            - No expliques conceptos básicos
            - Sé claro, sintético e inteligente

            Formato (respetar exacto):

            ## Resumen
            1 frase con el estado general del portfolio.

            ## Performance
            Para cada posición:
            - Inversión inicial vs valor actual
            - Ganancia/pérdida en ARS y %
            - Breve insight (1 línea)

            ## Real vs inflación
            Compará contra inflación (~140%) y dólar blue.
            Decí si ganó o perdió poder adquisitivo.

            ## Recomendaciones
            3 acciones concretas y directas (sin explicación teórica).

            Datos:

            Portfolio:
            {portfolioJson}

            Dólar (ARS):
            {ratesJson}

            Crypto:
            {cryptoJson}

            Objetivo:
            Detectar qué funciona, qué no, y cómo mejorar el rendimiento real del portfolio.
            """;

        var analysis = await this.geminiClient.GenerateContentAsync(prompt, cancellationToken);

        return new AnalysisResponse { Analysis = analysis, GeneratedAt = DateTime.UtcNow };
    }
}
