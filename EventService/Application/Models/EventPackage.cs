

namespace Application.Models;

public class EventPackage
{
    public string? EventId { get; set; }
    public Event? Event { get; set; }

    public string? PackageId { get; set; }
    public Package? Package { get; set; }
}
