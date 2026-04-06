using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/skills")]
[ApiController]
public class AdminSkillCatalogController : ControllerBase
{
    private readonly ISkillCatalogService _skillCatalogService;

    public AdminSkillCatalogController(ISkillCatalogService skillCatalogService)
    {
        _skillCatalogService = skillCatalogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _skillCatalogService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { Error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSkillCatalogItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _skillCatalogService.CreateAsync(User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillCatalogItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _skillCatalogService.UpdateAsync(User.GetUserId(), id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(new { Error = result.Error });
    }
}