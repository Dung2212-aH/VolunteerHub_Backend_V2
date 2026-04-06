using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Sponsor)]
[Route("api/sponsor/profile")]
[ApiController]
public class SponsorProfileController : ControllerBase
{
    private readonly ISponsorProfileService _sponsorProfileService;

    public SponsorProfileController(ISponsorProfileService sponsorProfileService)
    {
        _sponsorProfileService = sponsorProfileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await _sponsorProfileService.GetMyProfileAsync(User.GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateSponsorProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _sponsorProfileService.CreateProfileAsync(User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateSponsorProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _sponsorProfileService.UpdateProfileAsync(User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }
}