using SchunkToolsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// OpenAPI / Swagger
builder.Services.AddOpenApi();

// Typed HttpClient für den Python Tool-Generator-Service.
// Die BaseAddress kommt aus appsettings.json → "ToolGenerator:BaseUrl".
builder.Services.AddHttpClient<IToolGeneratorService, ToolGeneratorService>(client =>
{
    var baseUrl = builder.Configuration["ToolGenerator:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Konfiguration fehlt: 'ToolGenerator:BaseUrl' ist nicht gesetzt.");

    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");

    // Großzügiges Timeout: komplexe Modelle können mehrere Sekunden brauchen
    client.Timeout = TimeSpan.FromSeconds(120);
});

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI-JSON unter /openapi/v1.json, importierbar in Postman / Bruno
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
