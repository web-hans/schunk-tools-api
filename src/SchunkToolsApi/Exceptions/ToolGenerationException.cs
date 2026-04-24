namespace SchunkToolsApi.Exceptions;

/// <summary>
/// Wird geworfen, wenn der Python Tool-Generator-Service einen Fehler zurückgibt
/// oder nicht erreichbar ist. Controller fangen diese Exception und geben
/// einen 502 Bad Gateway zurück.
/// </summary>
public class ToolGenerationException(string message, Exception? innerException = null)
    : Exception(message, innerException);
