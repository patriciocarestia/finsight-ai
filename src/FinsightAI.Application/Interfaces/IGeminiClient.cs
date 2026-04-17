namespace FinsightAI.Application.Interfaces;

public interface IGeminiClient
{
    Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken);
}
