using System.Text.Json.Serialization;

namespace Application.Models;

public class Package
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("seatingArrangement")]
    public string SeatingArrangement { get; set; } = null!;

    [JsonPropertyName("placement")]
    public string? Placement { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
