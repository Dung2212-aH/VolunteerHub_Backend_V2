using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Organizer)]
[Route("api/organizer/sponsors")]
[ApiController]
public class OrganizerSponsorController : ControllerBase
{
    private readonly ISponsorManagementService _sponsorManagementService;

    public OrganizerSponsorController(ISponsorManagementService sponsorManagementService)
    {
        _sponsorManagementService = sponsorManagementService;
    }

    [HttpGet("events/{eventId:guid}/packages")]
    public async Task<IActionResult> GetEventPackages(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.GetEventPackagesAsync(User.GetUserId(), eventId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("events/{eventId:guid}/packages")]
    public async Task<IActionResult> CreatePackage(
        Guid eventId,
        [FromBody] CreateSponsorshipPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.CreatePackageAsync(User.GetUserId(), eventId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Package created successfully." }) : BadRequest(new { Error = result.Error });
    }

    [HttpPut("packages/{packageId:guid}")]
    public async Task<IActionResult> UpdatePackage(
        Guid packageId,
        [FromBody] UpdateSponsorshipPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.UpdatePackageAsync(User.GetUserId(), packageId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Package updated successfully." }) : BadRequest(new { Error = result.Error });
    }

    [HttpGet("events/{eventId:guid}/requests")]
    public async Task<IActionResult> GetEventSponsors(Guid eventId, CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.GetEventSponsorsAsync(User.GetUserId(), eventId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("requests/{eventSponsorId:guid}/review")]
    public async Task<IActionResult> ReviewEventSponsor(
        Guid eventSponsorId,
        [FromBody] ApproveRejectEventSponsorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.ReviewEventSponsorAsync(User.GetUserId(), eventSponsorId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Sponsor request reviewed successfully." }) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("contributions")]
    public async Task<IActionResult> RecordContribution(
        [FromBody] RecordContributionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sponsorManagementService.RecordContributionAsync(User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Contribution recorded successfully." }) : BadRequest(new { Error = result.Error });
    }
}