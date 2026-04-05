using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/organizers")]
[ApiController]
public class OrganizerModerationController : ControllerBase
{
    private readonly IOrganizerVerificationService _verificationService;

    public OrganizerModerationController(IOrganizerVerificationService verificationService)
    {
        _verificationService = verificationService;
    }

    private Guid GetAdminId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingVerifications(CancellationToken cancellationToken)
    {
        var result = await _verificationService.GetPendingVerificationsAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveOrganizer(Guid id, [FromBody] ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _verificationService.ApproveOrganizerAsync(GetAdminId(), id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectOrganizer(Guid id, [FromBody] ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _verificationService.RejectOrganizerAsync(GetAdminId(), id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> SuspendOrganizer(Guid id, [FromBody] ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _verificationService.SuspendOrganizerAsync(GetAdminId(), id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }
}
