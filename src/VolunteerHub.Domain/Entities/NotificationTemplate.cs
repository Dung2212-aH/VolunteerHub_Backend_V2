using VolunteerHub.Domain.Common;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
