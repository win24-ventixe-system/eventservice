
using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class CreateEventRequest
{
    public IFormFile? Image { get; set; } 

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    public List<CreatePackageRequest> Packages { get; set; } = [];

}
