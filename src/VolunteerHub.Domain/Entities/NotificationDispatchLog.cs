using VolunteerHub.Domain.Common;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Domain.Entities;

public class NotificationDispatchLog : BaseEntity
{
    public Guid NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}
