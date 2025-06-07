using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Entities;

public class EventPackageEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(Event))]
    public string EventId { get; set; } = null!;

    [JsonIgnore]
    public EventEntity Event { get; set; } = null!;


    [ForeignKey(nameof(Package))]
    public string? PackageId { get; set; }

    [JsonIgnore]
    public PackageEntity Package { get; set; } = null!;

}
