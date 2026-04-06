using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/complaints")]
[ApiController]
public class AdminComplaintController : ControllerBase
{
    private readonly IComplaintModerationService _complaintModerationService;

    public AdminComplaintController(IComplaintModerationService complaintModerationService)
    {
        _complaintModerationService = complaintModerationService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var result = await _complaintModerationService.GetPendingReportsAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveFeedbackReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _complaintModerationService.ResolveReportAsync(User.GetUserId(), id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }
}