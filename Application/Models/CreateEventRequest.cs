
using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class CreateEventRequest
{
    [Required]
    public IFormFile? Image { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public string? Location { get; set; }

    public List<CreatePackageRequest> Packages { get; set; } = [];

}
