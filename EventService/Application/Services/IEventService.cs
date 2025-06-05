

using Application.Models;
using Data.Entities;

namespace Presentation.Services;

public interface IEventService
{
    Task<EventResult<Event?>> GetEventAsync(string eventId);
    Task<EventResult<IEnumerable<Event>>> GetEventsAsync();

    Task<EventResult> CreateEventAsync(CreateEventRequest request);
    Task<EventResult> DeleteEventAsync(string eventId);

    Task<EventResult> UpdateEventAsync(string eventId, UpdateEventRequest request);

}
