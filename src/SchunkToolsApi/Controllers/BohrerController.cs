using Microsoft.AspNetCore.Mvc;
using SchunkToolsApi.Exceptions;
using SchunkToolsApi.Models;
using SchunkToolsApi.Services;

namespace SchunkToolsApi.Controllers;

[ApiController]
[Route("tools")]
public class BohrerController(IToolGeneratorService toolGenerator, ILogger<BohrerController> logger)
    : ControllerBase
{
    /// <summary>
    /// Generiert einen Spiralbohrer als STEP-Datei.
    /// Die Datei wird direkt als Download zurückgegeben.
    /// </summary>
    /// <param name="request">Bohrer-Parameter (Durchmesser, Längen, Schneidenwinkel)</param>
    [HttpPost("bohrer")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GenerateBohrer([FromBody] BohrerRequest request)
    {
        try
        {
            var stepBytes = await toolGenerator.GenerateAsync("bohrer", request);

            // Dateiname mit den Hauptmaßen für einfache Identifikation
            var filename = $"bohrer_d{request.D1}_L{request.L1}.step";
            return File(stepBytes, "application/octet-stream", filename);
        }
        catch (ToolGenerationException ex)
        {
            logger.LogError(ex, "Fehler bei der Bohrer-Generierung (d1={D1}, L1={L1})",
                request.D1, request.L1);

            // 502 = Python-Service hat geantwortet, aber mit einem Fehler
            return StatusCode(StatusCodes.Status502BadGateway, new ProblemDetails
            {
                Title = "Fehler bei der Werkzeug-Generierung",
                Detail = ex.Message,
                Status = StatusCodes.Status502BadGateway,
            });
        }
    }
}
