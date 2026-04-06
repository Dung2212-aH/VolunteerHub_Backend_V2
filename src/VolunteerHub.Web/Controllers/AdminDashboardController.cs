using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/dashboard")]
[ApiController]
public class AdminDashboardController : ControllerBase
{
    private readonly IImpactReportService _impactReportService;

    public AdminDashboardController(IImpactReportService impactReportService)
    {
        _impactReportService = impactReportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _impactReportService.GetDashboardAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }
}