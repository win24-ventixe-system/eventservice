
using Data.Entities;

namespace Application.Models;

public class UpdateEventRequest
{
    public int EventId { get; set; }
    public string Image { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    public List<PackageEntity> Packages { get; set; } = [];

}
