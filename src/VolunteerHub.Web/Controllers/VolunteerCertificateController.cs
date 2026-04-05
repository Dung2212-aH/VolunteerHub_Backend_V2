using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("api/certificates")]
[Authorize(Roles = "Volunteer")]
public class VolunteerCertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly IBadgeService _badgeService;
    private readonly IVolunteerProfileRepository _profileRepository;

    public VolunteerCertificateController(
        ICertificateService certificateService,
        IBadgeService badgeService,
        IVolunteerProfileRepository profileRepository)
    {
        _certificateService = certificateService;
        _badgeService = badgeService;
        _profileRepository = profileRepository;
    }

    private async Task<Guid?> ResolveProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var profile = await _profileRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        return profile?.Id;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCertificates(CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _certificateService.GetMyCertificatesAsync(profileId.Value, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("my/{id:guid}")]
    public async Task<IActionResult> GetMyCertificateById(Guid id, CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _certificateService.GetCertificateByIdAsync(profileId.Value, id, cancellationToken);
        if (!result.IsSuccess) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("badges")]
    public async Task<IActionResult> GetMyBadges(CancellationToken cancellationToken)
    {
        var profileId = await ResolveProfileIdAsync(cancellationToken);
        if (profileId == null) return BadRequest(new { Error = "Volunteer profile not found." });

        var result = await _badgeService.GetMyBadgesAsync(profileId.Value, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
