using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class AdminAuditService : IAdminAuditService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminAuditService(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(
        Guid adminUserId,
        string actionType,
        string entityType,
        Guid? entityId,
        string description,
        CancellationToken cancellationToken = default)
    {
        var log = new AdminActionLog
        {
            AdminUserId = adminUserId,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            Description = description
        };

        _adminRepository.AddAdminActionLog(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}