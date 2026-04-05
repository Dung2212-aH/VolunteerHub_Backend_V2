using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Notification;

namespace VolunteerHub.Application.Abstractions;

public interface INotificationService
{
    // ── Trigger Methods ──────────────────────────────────────────────
    Task SendWelcomeNotificationAsync(Guid userId, string email, string firstName, CancellationToken cancellationToken = default);
    Task SendEmailConfirmationNotificationAsync(Guid userId, string email, string confirmationLink, CancellationToken cancellationToken = default);
    Task SendPasswordResetNotificationAsync(Guid userId, string email, string resetLink, CancellationToken cancellationToken = default);
    Task NotifyApplicationSubmittedAsync(Guid userId, string eventTitle, Guid applicationId, CancellationToken cancellationToken = default);
    Task NotifyApplicationApprovedAsync(Guid userId, string eventTitle, Guid applicationId, CancellationToken cancellationToken = default);
    Task NotifyApplicationRejectedAsync(Guid userId, string eventTitle, Guid applicationId, string? reason, CancellationToken cancellationToken = default);
    Task NotifyCertificateIssuedAsync(Guid userId, string eventTitle, Guid certificateId, CancellationToken cancellationToken = default);
    Task NotifyBadgeAwardedAsync(Guid userId, string badgeName, Guid badgeId, CancellationToken cancellationToken = default);

    // ── Query Methods ────────────────────────────────────────────────
    Task<Result<List<NotificationListItemResponse>>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetMyUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
}
