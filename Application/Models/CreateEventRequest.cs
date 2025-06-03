
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class CreateEventRequest
{
    public string Image { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    //suggested by chat gpt so that the packages are created when creating the event
    public List<CreatePackageRequest> Packages { get; set; } = [];

}
