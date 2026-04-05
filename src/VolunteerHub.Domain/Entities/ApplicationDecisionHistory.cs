using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class ApplicationDecisionHistory : AuditableEntity
{
    public Guid EventApplicationId { get; set; }
    public EventApplication EventApplication { get; set; } = null!;

    public ApplicationStatus PreviousStatus { get; set; }
    public ApplicationStatus NewStatus { get; set; }

    public Guid ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;
}
