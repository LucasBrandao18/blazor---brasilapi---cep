using System.Text.Json.Serialization;

namespace BlazorWasmBrasilApiCep.Models;

public class Cep
{
    [JsonPropertyName("cep")]
    public string? Codigo { get; set; }

    [JsonPropertyName("state")]
    public string? Estado { get; set; }

    [JsonPropertyName("city")]
    public string? Cidade { get; set; }

    [JsonPropertyName("neighborhood")]
    public string? Bairro { get; set; }

    [JsonPropertyName("street")]
    public string? Logradouro { get; set; }

    [JsonPropertyName("service")]
    public string? Servico { get; set; }

    [JsonPropertyName("ibge")]
    public Ibge? Ibge { get; set; }
}
