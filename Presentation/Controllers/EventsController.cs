using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Services;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
{
    private readonly IEventService _eventService = eventService;

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
    public async Task<IActionResult> Create (CreateEventRequest request)
    {
        if(!ModelState.IsValid)
        return BadRequest(ModelState);

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
        if(result.Success)
            return NoContent();

        return NotFound(result.Error);  

    }
}
