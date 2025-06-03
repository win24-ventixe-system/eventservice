using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

public class Event
{
    public string Id { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime EventDate { get; set; }

    public string? Location { get; set; }
    public List<Package> Packages { get; set; } = [];
}
