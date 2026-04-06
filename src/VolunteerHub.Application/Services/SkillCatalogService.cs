using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class SkillCatalogService : ISkillCatalogService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IAdminAuditService _adminAuditService;
    private readonly IUnitOfWork _unitOfWork;

    public SkillCatalogService(
        IAdminRepository adminRepository,
        IAdminAuditService adminAuditService,
        IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _adminAuditService = adminAuditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SkillCatalogItemResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _adminRepository.GetSkillsAsync(cancellationToken);
        return Result.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<SkillCatalogItemResponse>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var items = await _adminRepository.GetActiveSkillsAsync(cancellationToken);
        return Result.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result> CreateAsync(Guid adminUserId, CreateSkillCatalogItemRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _adminRepository.GetSkillByCodeAsync(request.Code.Trim(), cancellationToken);
        if (existing != null)
            return Result.Failure(new Error("Admin.SkillCodeExists", "Skill code already exists."));

        var item = new SkillCatalogItem
        {
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            Category = request.Category?.Trim() ?? string.Empty,
            SortOrder = request.SortOrder,
            IsActive = true
        };

        _adminRepository.AddSkill(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _adminAuditService.LogAsync(
            adminUserId,
            "CreateSkill",
            nameof(SkillCatalogItem),
            item.Id,
            $"Created skill '{item.Code}' - '{item.Name}'.",
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Guid adminUserId, Guid id, UpdateSkillCatalogItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _adminRepository.GetSkillByIdAsync(id, cancellationToken);
        if (item == null)
            return Result.Failure(Error.NotFound);

        item.Name = request.Name.Trim();
        item.Category = request.Category?.Trim() ?? string.Empty;
        item.IsActive = request.IsActive;
        item.SortOrder = request.SortOrder;

        _adminRepository.UpdateSkill(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _adminAuditService.LogAsync(
            adminUserId,
            "UpdateSkill",
            nameof(SkillCatalogItem),
            item.Id,
            $"Updated skill '{item.Code}' - '{item.Name}', active={item.IsActive}.",
            cancellationToken);

        return Result.Success();
    }

    private static SkillCatalogItemResponse MapToResponse(SkillCatalogItem item)
    {
        return new SkillCatalogItemResponse
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            Category = item.Category,
            IsActive = item.IsActive,
            SortOrder = item.SortOrder
        };
    }
}