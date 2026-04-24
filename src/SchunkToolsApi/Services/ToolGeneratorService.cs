using System.Net.Http.Json;
using SchunkToolsApi.Exceptions;

namespace SchunkToolsApi.Services;

/// <summary>
/// Sendet Werkzeug-Parameter per HTTP an den Python FastAPI-Service
/// und leitet die STEP-Datei-Antwort an den Controller zurück.
/// </summary>
public class ToolGeneratorService(HttpClient httpClient, ILogger<ToolGeneratorService> logger)
    : IToolGeneratorService
{
    public async Task<byte[]> GenerateAsync(string endpoint, object parameters)
    {
        logger.LogInformation("Starte Werkzeug-Generierung: Endpunkt={Endpoint}", endpoint);

        HttpResponseMessage response;
        try
        {
            // Parameter als JSON an den Python-Service senden
            response = await httpClient.PostAsJsonAsync(endpoint, parameters);
        }
        catch (HttpRequestException ex)
        {
            // Python-Service nicht erreichbar (z. B. noch nicht gestartet)
            throw new ToolGenerationException(
                $"Python Tool-Generator-Service nicht erreichbar: {ex.Message}", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new ToolGenerationException(
                $"Python-Service antwortete mit {(int)response.StatusCode}: {body}");
        }

        var bytes = await response.Content.ReadAsByteArrayAsync();
        logger.LogInformation("Werkzeug-Generierung erfolgreich: {Bytes} Bytes empfangen", bytes.Length);

        return bytes;
    }
}
