using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Organizer)]
[Route("api/organizer/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    private Guid GetOrganizerId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
    {
        var organizerId = GetOrganizerId();
        var result = await _eventService.GetEventsByOrganizerAsync(organizerId, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(new { Error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _eventService.GetEventAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error == VolunteerHub.Application.Common.Error.NotFound 
                ? NotFound(new { Error = result.Error }) 
                : BadRequest(new { Error = result.Error });
        }
        
        if (result.Value.OrganizerId != GetOrganizerId())
        {
            return Forbid();
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        var organizerId = GetOrganizerId();
        var result = await _eventService.CreateEventAsync(organizerId, request, cancellationToken);
        
        return result.IsSuccess 
            ? Ok() 
            : BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var organizerId = GetOrganizerId();
        var result = await _eventService.UpdateEventAsync(organizerId, id, request, false, cancellationToken);
        
        if (result.IsFailure)
        {
            return result.Error == VolunteerHub.Application.Common.Error.NotFound 
                ? NotFound(new { Error = result.Error }) 
                : BadRequest(new { Error = result.Error });
        }

        return Ok();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishEvent(Guid id, CancellationToken cancellationToken)
    {
        var organizerId = GetOrganizerId();
        var result = await _eventService.PublishEventAsync(organizerId, id, cancellationToken);
        
        if (result.IsFailure)
        {
            return result.Error == VolunteerHub.Application.Common.Error.NotFound 
                ? NotFound(new { Error = result.Error }) 
                : BadRequest(new { Error = result.Error });
        }

        return Ok();
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> CloseEvent(Guid id, CancellationToken cancellationToken)
    {
        var organizerId = GetOrganizerId();
        var result = await _eventService.CloseEventAsync(organizerId, id, cancellationToken);
        
        if (result.IsFailure)
        {
            return result.Error == VolunteerHub.Application.Common.Error.NotFound 
                ? NotFound(new { Error = result.Error }) 
                : BadRequest(new { Error = result.Error });
        }

        return Ok();
    }
}
