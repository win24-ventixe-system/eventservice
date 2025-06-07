using System.Text.Json.Serialization;

namespace Application.Models;

public class CreatePackageRequest
{
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
