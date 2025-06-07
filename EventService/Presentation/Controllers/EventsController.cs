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
    [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)] // Returns a list of Event DTOs
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // In case of server error
    public async Task<IActionResult> GetAll()
    {
        var events = await _eventService.GetEventsAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)] // Returns a single Event DTO
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Event not found
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // In case of server error
    public async Task<IActionResult> GetEventAsync(string id)

    {
        var currentEvent = await _eventService.GetEventAsync(id);
        return currentEvent != null ? Ok(currentEvent) : NotFound();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Or StatusCodes.Status201Created if you return the created resource
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid request data (ModelState errors)
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error during creation
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
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Successful update, no content returned
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid request data or ID mismatch
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Event not found (if service returns it)
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error
    public async Task<IActionResult> Update(string id, [FromForm] UpdateEventRequest request)
    {
        //if (!ModelState.IsValid)
        //    return BadRequest(ModelState);
        if (!ModelState.IsValid)
        {
            // Log model state errors for debugging here as well
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    Console.WriteLine($"Model Error (Update): {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
        }

        if (id != request.EventId)
        {
            return BadRequest("Event ID in route does not match ID in request body.");
        }

        var result = await _eventService.UpdateEventAsync(id, request);
        return result.Success ? Ok() : StatusCode(500, result.Error);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Successfully deleted
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Event not found
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // In case of server error
    public async Task<IActionResult> Delete(string id)
    {

        var result = await _eventService.DeleteEventAsync(id);
        if (result.Success)
            return NoContent();

        return NotFound(result.Error);

    }
}
//using Application.Models;
//using Data.Handlers; // Assuming this contains your Result type or similar
//using Microsoft.AspNetCore.Mvc;
//using Presentation.Services; // Assuming IEventService is defined here
//using Microsoft.AspNetCore.Http; // For IFormFile and Request.Form
//using System.Linq; // For LINQ methods like .Any(), .Where, .GroupBy
//using System; // For Console.WriteLine and Convert.ChangeType (for robust parsing)
//using System.Text.Json; // Still useful if we need to parse sub-objects, but not for Packages directly now
//using System.Collections.Generic; // For List<T>

//namespace Presentation.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class EventsController(IEventService eventService) : ControllerBase
//{
//    private readonly IEventService _eventService = eventService;

//    [HttpGet]
//    [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> GetAll()
//    {
//        var events = await _eventService.GetEventsAsync();
//        return Ok(events);
//    }

//    [HttpGet("{id}")]
//    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> GetEventAsync(string id)
//    {
//        var currentEvent = await _eventService.GetEventAsync(id);
//        return currentEvent != null ? Ok(currentEvent) : NotFound();
//    }

//    [HttpPost]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> Create([FromForm] CreateEventRequest request)
//    {
//        // 1. Model State Validation for properties *other than* Packages
//        // The default binder will have populated simple properties, so this is still relevant.
//        if (!ModelState.IsValid)
//        {
//            foreach (var modelStateEntry in ModelState.Values)
//            {
//                foreach (var error in modelStateEntry.Errors)
//                {
//                    Console.WriteLine($"Model Error: {error.ErrorMessage}");
//                }
//            }
//            return BadRequest(ModelState);
//        }

//        // --- NEW DEFINITIVE SOLUTION FOR PACKAGES: Manual Indexed Binding ---
//        request.Packages.Clear(); // Ensure the list is empty before populating

//        // Group form fields by package index (e.g., "Packages[0]", "Packages[1]")
//        var packageGroups = Request.Form.Keys
//            .Where(key => key.StartsWith("Packages[") && key.Contains("]."))
//            .GroupBy(key =>
//            {
//                // Extract the index part, e.g., "Packages[0]" from "Packages[0].Title"
//                int start = key.IndexOf("[");
//                int end = key.IndexOf("]");
//                return key.Substring(start, end - start + 1); // Returns "[0]", "[1]", etc.
//            })
//            .OrderBy(g => g.Key); // Order by index for consistent processing

//        Console.WriteLine($"Found {packageGroups.Count()} package groups in Request.Form.");

//        foreach (var group in packageGroups)
//        {
//            var newPackage = new CreatePackageRequest();
//            bool packageHasData = false; // Flag to check if this package group actually had data

//            foreach (var key in group)
//            {
//                // Extract property name, e.g., "Title" from "Packages[0].Title"
//                var propertyName = key.Substring(key.IndexOf("].") + 2);
//                var value = Request.Form[key].FirstOrDefault(); // Get the value for that field

//                if (value != null)
//                {
//                    // Use reflection to set the property value dynamically
//                    var prop = typeof(CreatePackageRequest).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
//                    if (prop != null && prop.CanWrite)
//                    {
//                        try
//                        {
//                            // Convert the string value from the form to the property's actual type
//                            // Handle Decimal specifically if needed for culture-invariant parsing
//                            if (prop.PropertyType == typeof(decimal) && decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal priceValue))
//                            {
//                                prop.SetValue(newPackage, priceValue);
//                                packageHasData = true;
//                            }
//                            else if (prop.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
//                            {
//                                prop.SetValue(newPackage, intValue);
//                                packageHasData = true;
//                            }
//                            else if (prop.PropertyType == typeof(string)) // Direct string assignment
//                            {
//                                prop.SetValue(newPackage, value);
//                                if (!string.IsNullOrEmpty(value)) packageHasData = true;
//                            }
//                            else
//                            {
//                                // Fallback for other types using Convert.ChangeType
//                                var convertedValue = Convert.ChangeType(value, prop.PropertyType, System.Globalization.CultureInfo.InvariantCulture);
//                                prop.SetValue(newPackage, convertedValue);
//                                packageHasData = true;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"Error converting value for {propertyName} in CreatePackageRequest. Key: {key}, Value: '{value}'. Error: {ex.Message}");
//                            ModelState.AddModelError($"Packages[{group.Key}].{propertyName}", $"Invalid format for {propertyName}.");
//                        }
//                    }
//                    else if (prop == null)
//                    {
//                        Console.WriteLine($"Warning: Property '{propertyName}' not found on CreatePackageRequest for key: {key}");
//                    }
//                    else if (!prop.CanWrite)
//                    {
//                        Console.WriteLine($"Warning: Property '{propertyName}' on CreatePackageRequest is read-only for key: {key}");
//                    }
//                }
//            }

//            // Only add the package if it had any data successfully bound
//            if (packageHasData)
//            {
//                request.Packages.Add(newPackage);
//            }
//            else
//            {
//                Console.WriteLine($"Skipped adding an empty package from group: {group.Key}");
//            }
//        }
//        // --- END NEW DEFINITIVE SOLUTION FOR PACKAGES ---

//        Console.WriteLine($"Received Event Title: {request.Title}");
//        Console.WriteLine($"Number of Packages AFTER manual binding: {request.Packages.Count}");
//        foreach (var package in request.Packages)
//        {
//            Console.WriteLine($"Package Title: {package.Title ?? "N/A"}, Seating: {package.SeatingArrangement ?? "N/A"}, Placement: {package.Placement ?? "N/A"}, Price: {package.Price}, Currency: {package.Currency ?? "N/A"}");
//        }

//        // Only proceed if all model state (including manually added errors) is valid
//        if (!ModelState.IsValid)
//        {
//            return BadRequest(ModelState);
//        }

//        var result = await _eventService.CreateEventAsync(request);
//        return result.Success ? Ok() : StatusCode(500, result.Error);
//    }


//    [HttpPut("{id}")]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> Update(string id, [FromForm] UpdateEventRequest request)
//    {
//        if (!ModelState.IsValid)
//        {
//            foreach (var modelStateEntry in ModelState.Values)
//            {
//                foreach (var error in modelStateEntry.Errors)
//                {
//                    Console.WriteLine($"Model Error (Update): {error.ErrorMessage}");
//                }
//            }
//            return BadRequest(ModelState);
//        }

//        if (id != request.EventId)
//        {
//            return BadRequest("Event ID in route does not match ID in request body.");
//        }

//        // --- NEW DEFINITIVE SOLUTION FOR PACKAGES (UPDATE): Manual Indexed Binding ---
//        request.Packages.Clear();

//        var packageGroups = Request.Form.Keys
//            .Where(key => key.StartsWith("Packages[") && key.Contains("]."))
//            .GroupBy(key =>
//            {
//                int start = key.IndexOf("[");
//                int end = key.IndexOf("]");
//                return key.Substring(start, end - start + 1);
//            })
//            .OrderBy(g => g.Key);

//        Console.WriteLine($"Found {packageGroups.Count()} package groups in Request.Form for Update.");

//        foreach (var group in packageGroups)
//        {
//            var newPackage = new Application.Models.Package(); // Assuming this is your DTO for UpdateEventRequest's packages
//            bool packageHasData = false;

//            foreach (var key in group)
//            {
//                var propertyName = key.Substring(key.IndexOf("].") + 2);
//                var value = Request.Form[key].FirstOrDefault();

//                if (value != null)
//                {
//                    var prop = typeof(Application.Models.Package).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
//                    if (prop != null && prop.CanWrite)
//                    {
//                        try
//                        {
//                            if (prop.PropertyType == typeof(decimal) && decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal priceValue))
//                            {
//                                prop.SetValue(newPackage, priceValue);
//                                packageHasData = true;
//                            }
//                            else if (prop.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
//                            {
//                                prop.SetValue(newPackage, intValue);
//                                packageHasData = true;
//                            }
//                            else if (prop.PropertyType == typeof(string))
//                            {
//                                prop.SetValue(newPackage, value);
//                                if (!string.IsNullOrEmpty(value)) packageHasData = true;
//                            }
//                            else
//                            {
//                                var convertedValue = Convert.ChangeType(value, prop.PropertyType, System.Globalization.CultureInfo.InvariantCulture);
//                                prop.SetValue(newPackage, convertedValue);
//                                packageHasData = true;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"Error converting value for {propertyName} in Package (Update). Key: {key}, Value: '{value}'. Error: {ex.Message}");
//                            ModelState.AddModelError($"Packages[{group.Key}].{propertyName}", $"Invalid format for {propertyName}.");
//                        }
//                    }
//                    else if (prop == null)
//                    {
//                        Console.WriteLine($"Warning: Property '{propertyName}' not found on Package (Update) for key: {key}");
//                    }
//                    else if (!prop.CanWrite)
//                    {
//                        Console.WriteLine($"Warning: Property '{propertyName}' on Package (Update) is read-only for key: {key}");
//                    }
//                }
//            }
//            if (packageHasData)
//            {
//                request.Packages.Add(newPackage);
//            }
//            else
//            {
//                Console.WriteLine($"Skipped adding an empty package from group: {group.Key} (Update)");
//            }
//        }
//        // --- END NEW DEFINITIVE SOLUTION FOR PACKAGES (UPDATE) ---

//        Console.WriteLine($"Number of Packages after manual binding (Update): {request.Packages.Count}");

//        // Only proceed if all model state (including manually added errors) is valid
//        if (!ModelState.IsValid)
//        {
//            return BadRequest(ModelState);
//        }

//        var result = await _eventService.UpdateEventAsync(id, request);
//        return result.Success ? Ok() : StatusCode(500, result.Error);
//    }

//    [HttpDelete("{id}")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> Delete(string id)
//    {
//        var result = await _eventService.DeleteEventAsync(id);
//        if (result.Success)
//            return NoContent();

//        return NotFound(result.Error);
//    }
//}
