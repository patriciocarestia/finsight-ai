using System.Text.Json;
using System.Text.Json.Serialization;
using FinsightAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FinsightAI.Infrastructure.ExternalApis;

public class DolarApiClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DolarApiClient> logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DolarApiClient(HttpClient httpClient, ILogger<DolarApiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<IEnumerable<ExchangeRate>> FetchAllRatesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await this.httpClient.GetAsync("https://dolarapi.com/v1/dolares", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var dtos = JsonSerializer.Deserialize<List<DolarApiDto>>(json, JsonOptions) ?? [];

            return dtos.Select(d => new ExchangeRate
            {
                Type = MapType(d.Casa),
                Buy = d.Compra ?? 0,
                Sell = d.Venta ?? 0,
                RecordedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to fetch rates from DolarApi");
            return [];
        }
    }

    private static string MapType(string casa) => casa.ToLowerInvariant() switch
    {
        "oficial" => "oficial",
        "blue" => "blue",
        "bolsa" => "mep",
        "contadoconliqui" => "ccl",
        "cripto" => "cripto",
        _ => casa.ToLowerInvariant()
    };

    private record DolarApiDto
    {
        [JsonPropertyName("casa")]
        public string Casa { get; init; } = string.Empty;

        [JsonPropertyName("compra")]
        public decimal? Compra { get; init; }

        [JsonPropertyName("venta")]
        public decimal? Venta { get; init; }
    }
}
