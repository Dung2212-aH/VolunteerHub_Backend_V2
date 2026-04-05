using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Recognition;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("api/admin/certificates")]
[Authorize(Roles = "Admin")]
public class AdminCertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public AdminCertificateController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpPost("issue")]
    public async Task<IActionResult> IssueCertificate([FromBody] IssueCertificateRequest request, CancellationToken cancellationToken)
    {
        var result = await _certificateService.IssueCertificateAsync(request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/revoke")]
    public async Task<IActionResult> RevokeCertificate(Guid id, [FromBody] RevokeCertificateRequest request, CancellationToken cancellationToken)
    {
        var result = await _certificateService.RevokeCertificateAsync(id, request, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok();
    }
}
