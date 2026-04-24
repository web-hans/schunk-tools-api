using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchunkToolsApi.Models;

/// <summary>
/// Eingehende Parameter für die Bohrer-Generierung.
/// Alle Längenangaben in Millimeter.
/// </summary>
public record BohrerRequest
{
    /// <summary>Bohrdurchmesser (Schneidendurchmesser), z. B. 2.0 mm</summary>
    [Required]
    [Range(0.1, 100, ErrorMessage = "d1 muss zwischen 0.1 und 100 mm liegen.")]
    [JsonPropertyName("d1")]
    public double D1 { get; init; }

    /// <summary>Gesamtlänge des Bohrers, z. B. 57.0 mm</summary>
    [Required]
    [Range(1, 1000, ErrorMessage = "L1 muss zwischen 1 und 1000 mm liegen.")]
    [JsonPropertyName("L1")]
    public double L1 { get; init; }

    /// <summary>Schneidenlänge (Länge der Schneiden), z. B. 16.0 mm</summary>
    [Required]
    [Range(1, 500, ErrorMessage = "L2 muss zwischen 1 und 500 mm liegen.")]
    [JsonPropertyName("L2")]
    public double L2 { get; init; }

    /// <summary>Spannutenlänge (Länge des Bereichs mit Bohrdurchmesser), z. B. 21.0 mm</summary>
    [Required]
    [Range(1, 500, ErrorMessage = "L3 muss zwischen 1 und 500 mm liegen.")]
    [JsonPropertyName("L3")]
    public double L3 { get; init; }

    /// <summary>Schaftdurchmesser (i. d. R. größer als d1), z. B. 3.0 mm</summary>
    [Required]
    [Range(0.1, 100, ErrorMessage = "d2 muss zwischen 0.1 und 100 mm liegen.")]
    [JsonPropertyName("d2")]
    public double D2 { get; init; }

    /// <summary>Schneidenwinkel (Kegelwinkel) an der Spitze in Grad. Standard: 140°</summary>
    [Range(60, 180, ErrorMessage = "Schneidenwinkel muss zwischen 60 und 180 Grad liegen.")]
    [JsonPropertyName("schneidenwinkel")]
    public double Schneidenwinkel { get; init; } = 140.0;
}
