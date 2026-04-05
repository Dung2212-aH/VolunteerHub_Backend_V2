using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Area("Organizer")]
[Route("api/organizer/attendance")]
[ApiController]
[Authorize(Roles = AppRoles.Organizer)]
public class OrganizerAttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public OrganizerAttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("shifts/{shiftId:guid}/assign")]
    public async Task<IActionResult> AssignVolunteer(Guid shiftId, [FromBody] AssignShiftRequest request, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _attendanceService.AssignVolunteerAsync(organizerId, shiftId, request.VolunteerProfileId, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("events/{eventId:guid}/shifts")]
    public async Task<IActionResult> CreateShift(Guid eventId, [FromBody] CreateShiftRequest request, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _attendanceService.CreateShiftAsync(organizerId, eventId, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("events/{eventId:guid}")]
    public async Task<IActionResult> GetEventAttendance(Guid eventId, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _attendanceService.GetEventAttendanceAsync(organizerId, eventId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("shifts/{shiftId:guid}/override")]
    public async Task<IActionResult> ManualOverride(Guid shiftId, [FromBody] ManualOverrideRequest request, CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();
        var result = await _attendanceService.ManualOverrideAsync(organizerId, shiftId, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
