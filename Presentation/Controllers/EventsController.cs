using Application.Models;
using Data.Handlers;
using Microsoft.AspNetCore.Mvc;
using Presentation.Services;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController(IEventService eventService, IFileHandler fileHandler) : ControllerBase
{
    private readonly IEventService _eventService = eventService;
    private readonly IFileHandler _fileHandler = fileHandler;



    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _eventService.GetEventsAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventAsync(string id)
    {
        var currentEvent = await _eventService.GetEventAsync(id);
        return currentEvent != null ? Ok(currentEvent) : NotFound();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateEventRequest request)
    {
        //if (!ModelState.IsValid)
        //    return BadRequest(ModelState);
        if (!ModelState.IsValid)
        {
            // Log model state errors for debugging
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    Console.WriteLine($"Model Error: {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
        }

        var result = await _eventService.CreateEventAsync(request);
        return result.Success ? Ok() : StatusCode(500, result.Error);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _eventService.UpdateEventAsync(id, request);

        return result.Success ? Ok() : StatusCode(500, result.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {

        var result = await _eventService.DeleteEventAsync(id);
        if (result.Success)
            return NoContent();

        return NotFound(result.Error);

    }
}
