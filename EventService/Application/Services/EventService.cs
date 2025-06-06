using Application.Models;
using Data.Contexts;
using Data.Entities;
using Data.Handlers;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Presentation.Services;
public class EventService(IEventRepository eventRepository, IFileHandler fileHandler) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IFileHandler _fileHandler = fileHandler;


    public async Task<EventResult> CreateEventAsync(CreateEventRequest request)
    {
        try
        {
            string? imageFileUri = null;

            if (request.Image != null)
            {
                imageFileUri = await _fileHandler.UploadFileAsync(request.Image);
            }

            var eventEntity = new EventEntity
            {

                Image = imageFileUri ?? string.Empty,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                EventDate = request.EventDate

            };

            eventEntity.EventsPackages ??= new List<EventPackageEntity>();

                if (request.Packages != null)
                {
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
                }
            Console.WriteLine($"Event to save: {JsonSerializer.Serialize(eventEntity)}");
            var result = await _eventRepository.AddAsync(eventEntity);
            return result.Success
               ? new EventResult { Success = true }
               : new EventResult { Success = false, Error = result.Error };

        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            Console.WriteLine("Inner: " + ex.InnerException?.Message);
            Console.WriteLine("Inner.Inner: " + ex.InnerException?.InnerException?.Message);

            return new EventResult
            {
                Success = false,
                Error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message
            };
        }

        //catch (Exception ex)
        //{
        //    return new EventResult
        //    {
        //        Success = false,
        //        //Error = ex.Message
        //        Error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message

        //    };
        //}
    }
    public async Task<EventResult<IEnumerable<Event>>> GetEventsAsync()
    {
        var result = await _eventRepository.GetAllAsync();
        var events = result.Result?.Select(e => new Event
        {
            Id = e.Id,
            Image = e.Image!,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            EventDate = e.EventDate,
            Packages = e.EventsPackages.Select(ep => new Package
            {
                Id = ep.Package.Id.ToString(),
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
        var result = await _eventRepository.GetAsync(x => x.Id == eventId);



        if (result.Success && result.Result != null)
        {
            var entity = result.Result;

            var currentEvent = new Event
            {
                Id = result.Result.Id,
                Image = result.Result.Image!,
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

            // Update basic event properties
            entity.Title = request.Title ?? entity.Title;
            entity.Description = request.Description ?? entity.Description;
            entity.EventDate = request.EventDate;
            entity.Location = request.Location ?? entity.Location;
            if (request.Image != null) // Check if a NEW image file was provided
            {
                entity.Image = await _fileHandler.UploadFileAsync(request.Image);
               
            }
            // Sync Packages

            entity.EventsPackages ??= new List<EventPackageEntity>();
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
                    entity.EventsPackages.Add(new EventPackageEntity
                    {
                        Event = entity,
                        Package = newPackage
                    });
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
            if (!result.Success || result.Result == null)
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
