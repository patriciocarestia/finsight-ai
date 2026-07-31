namespace DolarEnVivo.Application.Exceptions;

public class GeminiException : Exception
{
    public GeminiException(string message)
        : base(message) { }
}
