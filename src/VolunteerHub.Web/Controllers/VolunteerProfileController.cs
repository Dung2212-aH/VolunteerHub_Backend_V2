using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Web.Controllers;

[Area("Volunteer")]
[Route("api/[area]/profile")]
[ApiController]
[Authorize(Roles = AppRoles.Volunteer)]
public class VolunteerProfileController : ControllerBase
{
    private readonly IVolunteerProfileService _profileService;

    public VolunteerProfileController(IVolunteerProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _profileService.GetProfileAsync(userId, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound(new { result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _profileService.CreateProfileAsync(userId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Error });
        }

        return Ok(new { Message = "Profile created successfully." });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Error });
        }

        return Ok(new { Message = "Profile updated successfully." });
    }
}
