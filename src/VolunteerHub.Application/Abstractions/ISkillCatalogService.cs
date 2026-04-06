using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface ISkillCatalogService
{
    Task<Result<List<SkillCatalogItemResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<List<SkillCatalogItemResponse>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(Guid adminUserId, CreateSkillCatalogItemRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid adminUserId, Guid id, UpdateSkillCatalogItemRequest request, CancellationToken cancellationToken = default);
}