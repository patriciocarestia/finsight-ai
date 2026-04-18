using System.Net;
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
    private readonly string primaryModelId;
    private const string FallbackModelId = "gemini-2.0-flash";
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    public GeminiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiClient> logger
    )
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.httpClient = httpClient;
        this.logger = logger;
        this.apiKey =
            configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
        this.primaryModelId = configuration["Gemini:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<string> GenerateContentAsync(
        string prompt,
        CancellationToken cancellationToken
    )
    {
        var modelsToTry =
            this.primaryModelId == FallbackModelId
                ? new[] { this.primaryModelId }
                : new[] { this.primaryModelId, FallbackModelId };

        foreach (var model in modelsToTry)
        {
            var text = await TryWithRetriesAsync(model, prompt, cancellationToken);
            if (text is not null)
                return text;
        }

        return "El análisis no está disponible en este momento. Por favor intentá más tarde.";
    }

    private async Task<string?> TryWithRetriesAsync(
        string model,
        string prompt,
        CancellationToken cancellationToken
    )
    {
        var url = $"{BaseUrl}/{model}:generateContent?key={this.apiKey}";
        var requestBody = BuildRequestBody(prompt);

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                var response = await this.httpClient.PostAsJsonAsync(
                    url,
                    requestBody,
                    cancellationToken
                );
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (
                    response.StatusCode == HttpStatusCode.ServiceUnavailable
                    || response.StatusCode == HttpStatusCode.TooManyRequests
                )
                {
                    if (attempt < 3)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
                        continue;
                    }

                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError(
                        "Gemini API error. Status: {Status}, Model: {Model}, Body: {Body}",
                        response.StatusCode,
                        model,
                        responseBody[..Math.Min(300, responseBody.Length)]
                    );
                    return null;
                }

                var result = JsonSerializer.Deserialize<GeminiResponse>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var text = result
                    ?.Candidates?.FirstOrDefault()
                    ?.Content?.Parts?.FirstOrDefault()
                    ?.Text;

                if (string.IsNullOrWhiteSpace(text))
                    return null;

                return text;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Exception calling Gemini. Model: {Model}, Attempt: {Attempt}",
                    model,
                    attempt
                );
                if (attempt == 3)
                    return null;
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
            }
        }

        return null;
    }

    private static object BuildRequestBody(string prompt) =>
        new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { temperature = 0.7, maxOutputTokens = 65536 },
        };

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
