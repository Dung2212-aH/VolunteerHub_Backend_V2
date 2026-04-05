using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Rating;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("api/feedback")]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Submit a complaint/report against another user within an event context.
    /// </summary>
    [HttpPost("report")]
    public async Task<IActionResult> SubmitReport([FromBody] CreateFeedbackReportRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _feedbackService.SubmitReportAsync(userId, request, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Report submitted." }) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Get all feedback reports submitted by the current user.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReports(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _feedbackService.GetMyReportsAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }
}
