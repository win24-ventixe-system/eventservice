
using Microsoft.AspNetCore.Http;

namespace Application.Models;

public class UpdateEventRequest
{
    public string? EventId { get; set; }
    public IFormFile? Image { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    public List<Package> Packages { get; set; } = [];

}
