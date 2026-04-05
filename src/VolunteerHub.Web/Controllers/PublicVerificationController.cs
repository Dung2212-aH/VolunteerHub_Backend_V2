using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("certificate/verify")]
public class PublicVerificationController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public PublicVerificationController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpGet("{verificationCode}")]
    public async Task<IActionResult> Verify(string verificationCode, CancellationToken cancellationToken)
    {
        var result = await _certificateService.VerifyCertificateAsync(verificationCode, cancellationToken);
        if (!result.IsSuccess) return NotFound(result.Error); // Or BadRequest depending on convention

        return Ok(result.Value);
    }
}
