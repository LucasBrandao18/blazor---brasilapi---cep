using System.Text.Json.Serialization;

namespace BlazorWasmBrasilApiCep.Models;

public class Ibge
{
    [JsonPropertyName("city")]
    public string? Cidade { get; set; }

    [JsonPropertyName("state")]
    public string? Estado { get; set; }
}
