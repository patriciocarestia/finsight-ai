namespace FinsightAI.Application.DTOs;

public record AnalysisResponse
{
    public string Analysis { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}
