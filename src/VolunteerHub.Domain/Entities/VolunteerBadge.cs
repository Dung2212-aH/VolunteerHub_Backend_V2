using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class VolunteerBadge : AuditableEntity
{
    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;

    public Guid BadgeId { get; set; }
    public Badge Badge { get; set; } = null!;

    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    public string AwardReason { get; set; } = string.Empty;
    public string SourceReference { get; set; } = string.Empty;
}
