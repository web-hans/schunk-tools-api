namespace SchunkToolsApi.Services;

/// <summary>
/// Abstraktion für die Kommunikation mit dem Python Tool-Generator-Service.
/// Alle Werkzeug-Controller nutzen ausschließlich dieses Interface,
/// damit der Python-Service in Tests einfach gemockt werden kann.
/// </summary>
public interface IToolGeneratorService
{
    /// <summary>
    /// Sendet die Werkzeug-Parameter an den Python-Service und gibt
    /// die generierte STEP-Datei als Byte-Array zurück.
    /// </summary>
    /// <param name="endpoint">Endpunkt des Python-Service, z. B. "bohrer"</param>
    /// <param name="parameters">Parameterobjekt, wird als JSON serialisiert</param>
    /// <returns>Rohe Bytes der STEP-Datei</returns>
    /// <exception cref="Exceptions.ToolGenerationException">
    /// Wird geworfen, wenn der Python-Service einen Fehler meldet.
    /// </exception>
    Task<byte[]> GenerateAsync(string endpoint, object parameters);
}
