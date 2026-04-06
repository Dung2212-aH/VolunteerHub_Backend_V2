using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class AdminActionLog : AuditableEntity
{
    public Guid AdminUserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
}