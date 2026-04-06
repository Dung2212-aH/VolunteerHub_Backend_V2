using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Sponsor)]
[Route("api/sponsor/events")]
[ApiController]
public class SponsorEventController : ControllerBase
{
    private readonly ISponsorProfileService _sponsorProfileService;

    public SponsorEventController(ISponsorProfileService sponsorProfileService)
    {
        _sponsorProfileService = sponsorProfileService;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestSponsorEvent([FromBody] SponsorEventRequest request, CancellationToken cancellationToken)
    {
        var result = await _sponsorProfileService.RequestSponsorEventAsync(User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }

    [HttpGet("my-sponsorships")]
    public async Task<IActionResult> GetMySponsorships(CancellationToken cancellationToken)
    {
        var result = await _sponsorProfileService.GetMyEventSponsorsAsync(User.GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }
}