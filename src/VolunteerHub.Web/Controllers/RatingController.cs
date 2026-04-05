using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Rating;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("api/ratings")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    /// <summary>
    /// Volunteer rates an organizer after participating in a completed event.
    /// </summary>
    [HttpPost("organizer")]
    [Authorize(Roles = AppRoles.Volunteer)]
    public async Task<IActionResult> RateOrganizer([FromBody] CreateRatingRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _ratingService.SubmitVolunteerRatingAsync(userId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Rating submitted." }) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Organizer rates a volunteer after a completed event.
    /// </summary>
    [HttpPost("volunteer")]
    [Authorize(Roles = AppRoles.Organizer)]
    public async Task<IActionResult> RateVolunteer([FromBody] CreateRatingRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _ratingService.SubmitOrganizerRatingAsync(userId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Rating submitted." }) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Get all ratings involving the current user (given and received).
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRatings(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _ratingService.GetMyRatingsAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Organizer gets all ratings for a specific event they own.
    /// </summary>
    [HttpGet("event/{eventId:guid}")]
    [Authorize(Roles = AppRoles.Organizer)]
    public async Task<IActionResult> GetRatingsByEvent(Guid eventId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _ratingService.GetRatingsByEventAsync(userId, eventId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Get aggregated rating summary for any user (public query).
    /// </summary>
    [HttpGet("summary/{targetUserId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRatingSummary(Guid targetUserId, CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetRatingSummaryAsync(targetUserId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }
}
