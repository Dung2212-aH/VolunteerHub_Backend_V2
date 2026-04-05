using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Area("Volunteer")]
[Route("api/[area]/applications")]
[ApiController]
[Authorize(Roles = AppRoles.Volunteer)]
public class VolunteerApplicationController : ControllerBase
{
    private readonly IEventApplicationService _applicationService;
    private readonly IVolunteerProfileRepository _profileRepository;

    public VolunteerApplicationController(
        IEventApplicationService applicationService,
        IVolunteerProfileRepository profileRepository)
    {
        _applicationService = applicationService;
        _profileRepository = profileRepository;
    }

    private async Task<Guid?> ResolveProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var profile = await _profileRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        return profile?.Id;
    }

    [HttpPost]
    public async Task<IActionResult> ApplyToEvent([FromBody] ApplyToEventRequest request, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found. Create a profile first." });

        var result = await _applicationService.ApplyAsync(profileId.Value, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Application submitted." }) : BadRequest(new { result.Error });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications(CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _applicationService.GetMyApplicationsAsync(profileId.Value, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }

    [HttpGet("my/{id}")]
    public async Task<IActionResult> GetApplicationDetails(Guid id, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _applicationService.GetApplicationAsync(profileId.Value, id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> WithdrawApplication(Guid id, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _applicationService.WithdrawAsync(profileId.Value, id, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Application withdrawn." }) : BadRequest(new { result.Error });
    }
}
