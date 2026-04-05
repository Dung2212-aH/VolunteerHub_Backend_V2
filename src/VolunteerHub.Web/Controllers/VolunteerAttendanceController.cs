using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Area("Volunteer")]
[Route("api/volunteer/attendance")]
[ApiController]
[Authorize(Roles = AppRoles.Volunteer)]
public class VolunteerAttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IVolunteerProfileRepository _profileRepository;

    public VolunteerAttendanceController(
        IAttendanceService attendanceService,
        IVolunteerProfileRepository profileRepository)
    {
        _attendanceService = attendanceService;
        _profileRepository = profileRepository;
    }

    private async Task<Guid?> ResolveProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var profile = await _profileRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        return profile?.Id;
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _attendanceService.CheckInAsync(profileId.Value, request, cancellationToken);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _attendanceService.CheckOutAsync(profileId.Value, request, cancellationToken);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAttendance(CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _attendanceService.GetMyAttendanceAsync(profileId.Value, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
