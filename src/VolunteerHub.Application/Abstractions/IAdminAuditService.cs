namespace VolunteerHub.Application.Abstractions;

public interface IAdminAuditService
{
    Task LogAsync(
        Guid adminUserId,
        string actionType,
        string entityType,
        Guid? entityId,
        string description,
        CancellationToken cancellationToken = default);
}