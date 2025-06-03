using Data.Entities;
using Data.Repositories;
using Application.Models;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Services;
public class EventService(IEventRepository eventRepository) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;

    public async Task<EventResult> CreateEventAsync(CreateEventRequest request)
    {
        try
        {
            var eventEntity = new EventEntity
            {

                Image = request.Image,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                EventDate = request.EventDate

            };
            //with chat gpt help
            eventEntity.EventsPackages = [];

            foreach (var package in request.Packages)
            {
                var packageEntity = new PackageEntity
                {
                    Title = package.Title,
                    SeatingArrangement = package.SeatingArrangement,
                    Placement = package.Placement,
                    Price = package.Price,
                    Currency = package.Currency
                };
                var eventPackage = new EventPackageEntity
                {
                    Event = eventEntity,
                    Package = packageEntity
                };

                eventEntity.EventsPackages.Add(eventPackage);
            }
            var result = await _eventRepository.AddAsync(eventEntity);
            return result.Success
               ? new EventResult { Success = true }
               : new EventResult { Success = false, Error = result.Error };

        }

        catch (Exception ex)
        {
            return new EventResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
    public async Task<EventResult<IEnumerable<Event>>> GetEventsAsync()
    {
        var result = await _eventRepository.GetAllAsync();
        var events = result.Result?.Select(e => new Event
        {
            Id = e.Id,
            Image = e.Image,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            EventDate = e.EventDate,
            Packages = e.EventsPackages.Select(ep => new Package
            {
                Id = ep.Package.Id,
                Title = ep.Package.Title,
                SeatingArrangement = ep.Package.SeatingArrangement,
                Placement = ep.Package.Placement,
                Price = ep.Package.Price,
                Currency = ep.Package.Currency
            }).ToList()

        });
        return new EventResult<IEnumerable<Event>> { Success = true, Result = events };
    }
   
    public async Task<EventResult<Event?>> GetEventAsync(string eventId)
    {
        var result = await _eventRepository.GetAsync(x=> x.Id == eventId);



        if (result.Success && result.Result != null)
        {
            var entity = result.Result;

            var currentEvent = new Event
            {
                Id = result.Result.Id,
                Image = result.Result.Image,
                Title = result.Result.Title,
                Description = result.Result.Description,
                Location = result.Result.Location,
                EventDate = result.Result.EventDate,
                Packages = entity.EventsPackages.Select(ep => new Package
                {
                    Id = ep.Package.Id,
                    Title = ep.Package.Title,
                    SeatingArrangement = ep.Package.SeatingArrangement,
                    Placement = ep.Package.Placement,
                    Price = ep.Package.Price,
                    Currency = ep.Package.Currency
                }).ToList()
            };


            return new EventResult<Event?> { Success = true, Result = currentEvent };
        }

        return new EventResult<Event?> { Success = false, Error = "Event Not found" };

    }
    public async Task<EventResult> UpdateEventAsync(string eventId, UpdateEventRequest request)
    {
        try
        {
            var result = await _eventRepository.GetAsync(e => e.Id == eventId);

            if (!result.Success || result.Result == null)
                return new EventResult { Success = false, Error = "Event not found" };

            var entity = result.Result;

            // Update event properties
            entity.Image = request.Image;
            entity.Title = request.Title ?? entity.Title;
            entity.Description = request.Description ?? entity.Description;
            entity.EventDate = request.EventDate;
            entity.Location = request.Location ?? entity.Location;

            // Sync Packages
            var existingEventPackages = entity.EventsPackages.ToList();
            // 1. Update or add incoming packages
            foreach (var incoming in request.Packages)
            {
                var existingEp = existingEventPackages
                    .FirstOrDefault(ep => ep.Package.Id == incoming.Id);

                if (existingEp != null)
                {
                    // Update existing
                    existingEp.Package.Title = incoming.Title;
                    existingEp.Package.SeatingArrangement = incoming.SeatingArrangement;
                    existingEp.Package.Placement = incoming.Placement;
                    existingEp.Package.Price = incoming.Price;
                    existingEp.Package.Currency = incoming.Currency;
                }
                else
                {
                    // New package
                    var newPackage = new PackageEntity
                    {
                        Title = incoming.Title,
                        SeatingArrangement = incoming.SeatingArrangement,
                        Placement = incoming.Placement,
                        Price = incoming.Price,
                        Currency = incoming.Currency
                    };
             
                }

                
            }
            var updateResult = await _eventRepository.UpdateAsync(entity);

            return updateResult.Success
                ? new EventResult { Success = true }
                : new EventResult { Success = false, Error = updateResult.Error };
        }


        catch (Exception ex)
        {
            return new EventResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }


    public async Task<EventResult> DeleteEventAsync(string eventId)
    {
        try
        {
            var result = await _eventRepository.GetAsync(x => x.Id == eventId);
            if (!result.Success || result.Result != null)
                return new EventResult { Success = false, Error = "Event not found" };

            var deleteResult = await _eventRepository.DeleteAsync(result.Result!);

            return deleteResult.Success

            ? new EventResult { Success = true }
            : new EventResult { Success = false, Error = deleteResult.Error };
        }
        catch (Exception ex)
        {
            return new EventResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
}
