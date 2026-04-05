using VolunteerHub.Domain.Common;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Domain.Entities;

public class FeedbackReport : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
