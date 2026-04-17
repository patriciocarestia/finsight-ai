using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinsightAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FinsightAI.Infrastructure.AI;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<GeminiClient> logger;
    private readonly string apiKey;

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public GeminiClient(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.httpClient = httpClient;
        this.logger = logger;
        this.apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
    }

    public async Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var url = $"{BaseUrl}?key={this.apiKey}";
            var response = await this.httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeminiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                ?? "No se pudo generar el análisis.";
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to call Gemini API");
            return "El análisis no está disponible en este momento. Por favor intentá más tarde.";
        }
    }

    private record GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; init; }
    }

    private record Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; init; }
    }

    private record Content
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; init; }
    }

    private record Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; init; }
    }
}
