using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Area("Organizer")]
[Route("api/[area]/applications")]
[ApiController]
[Authorize(Roles = AppRoles.Organizer)]
public class OrganizerApplicationReviewController : ControllerBase
{
    private readonly IApplicationReviewService _reviewService;

    public OrganizerApplicationReviewController(IApplicationReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("~/api/[area]/events/{eventId:guid}/applications")]
    public async Task<IActionResult> GetEventApplications(Guid eventId, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.GetEventApplicationsAsync(organizerId, eventId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApplicationDetails(Guid id, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.GetApplicationDetailsAsync(organizerId, id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveApplication(Guid id, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.ApproveAsync(organizerId, id, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Application approved." }) : BadRequest(new { result.Error });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectApplication(Guid id, [FromBody] ReviewApplicationRequest request, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.RejectAsync(organizerId, id, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Application rejected." }) : BadRequest(new { result.Error });
    }

    [HttpPost("{id:guid}/waitlist")]
    public async Task<IActionResult> WaitlistApplication(Guid id, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.WaitlistAsync(organizerId, id, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Application waitlisted." }) : BadRequest(new { result.Error });
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddReviewNote(Guid id, [FromBody] AddReviewNoteRequest request, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _reviewService.AddNoteAsync(organizerId, id, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Note added." }) : BadRequest(new { result.Error });
    }
}
