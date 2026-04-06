using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/sponsors")]
[ApiController]
public class AdminSponsorModerationController : ControllerBase
{
    private readonly ISponsorManagementService _sponsorManagementService;

    public AdminSponsorModerationController(ISponsorManagementService sponsorManagementService)
    {
        _sponsorManagementService = sponsorManagementService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingProfiles(CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.GetPendingSponsorProfilesAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/review")]
    public async Task<IActionResult> ReviewProfile(Guid id, [FromBody] ApproveSponsorProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.ReviewSponsorProfileAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }
}