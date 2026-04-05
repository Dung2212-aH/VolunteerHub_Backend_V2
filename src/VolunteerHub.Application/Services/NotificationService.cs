using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Notification;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository repository,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    // ── Trigger Methods ──────────────────────────────────────────────

    public async Task SendWelcomeNotificationAsync(Guid userId, string email, string firstName, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "FirstName", firstName }
        };

        await CreateAndDispatchAsync(userId, email, NotificationType.Welcome, "WELCOME", placeholders, null, null, cancellationToken);
    }

    public async Task SendEmailConfirmationNotificationAsync(Guid userId, string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "ConfirmationLink", confirmationLink }
        };

        // Email-only: security-sensitive, no in-app record needed
        await CreateAndDispatchAsync(userId, email, NotificationType.EmailConfirmation, "EMAIL_CONFIRMATION", placeholders, null, null, cancellationToken, emailOnly: true);
    }

    public async Task SendPasswordResetNotificationAsync(Guid userId, string email, string resetLink, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "ResetLink", resetLink }
        };

        // Email-only: security-sensitive
        await CreateAndDispatchAsync(userId, email, NotificationType.PasswordReset, "PASSWORD_RESET", placeholders, null, null, cancellationToken, emailOnly: true);
    }

    public async Task NotifyApplicationSubmittedAsync(Guid userId, string eventTitle, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "EventTitle", eventTitle }
        };

        await CreateAndDispatchAsync(userId, null, NotificationType.ApplicationSubmitted, "APPLICATION_SUBMITTED", placeholders, "EventApplication", applicationId, cancellationToken);
    }

    public async Task NotifyApplicationApprovedAsync(Guid userId, string eventTitle, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "EventTitle", eventTitle }
        };

        await CreateAndDispatchAsync(userId, null, NotificationType.ApplicationApproved, "APPLICATION_APPROVED", placeholders, "EventApplication", applicationId, cancellationToken);
    }

    public async Task NotifyApplicationRejectedAsync(Guid userId, string eventTitle, Guid applicationId, string? reason, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "EventTitle", eventTitle },
            { "Reason", reason ?? "No reason provided." }
        };

        await CreateAndDispatchAsync(userId, null, NotificationType.ApplicationRejected, "APPLICATION_REJECTED", placeholders, "EventApplication", applicationId, cancellationToken);
    }

    public async Task NotifyCertificateIssuedAsync(Guid userId, string eventTitle, Guid certificateId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "EventTitle", eventTitle }
        };

        await CreateAndDispatchAsync(userId, null, NotificationType.CertificateIssued, "CERTIFICATE_ISSUED", placeholders, "Certificate", certificateId, cancellationToken);
    }

    public async Task NotifyBadgeAwardedAsync(Guid userId, string badgeName, Guid badgeId, CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "BadgeName", badgeName }
        };

        await CreateAndDispatchAsync(userId, null, NotificationType.BadgeAwarded, "BADGE_AWARDED", placeholders, "Badge", badgeId, cancellationToken);
    }

    // ── Query Methods ────────────────────────────────────────────────

    public async Task<Result<List<NotificationListItemResponse>>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _repository.GetByUserIdAsync(userId, cancellationToken);
        var response = notifications.Select(n => new NotificationListItemResponse
        {
            Id = n.Id,
            Type = n.Type.ToString(),
            Title = n.Title,
            Status = n.Status.ToString(),
            IsRead = n.Status == NotificationStatus.Read,
            CreatedAt = n.CreatedAt
        }).ToList();

        return Result.Success(response);
    }

    public async Task<Result<int>> GetMyUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var count = await _repository.GetUnreadCountAsync(userId, cancellationToken);
        return Result.Success(count);
    }

    public async Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null || notification.UserId != userId)
            return Result.Failure(Error.NotFound);

        if (notification.Status == NotificationStatus.Read)
            return Result.Success(); // Already read, idempotent

        notification.Status = NotificationStatus.Read;
        notification.ReadAt = DateTime.UtcNow;

        _repository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // ── Private Core Logic ───────────────────────────────────────────

    private async Task CreateAndDispatchAsync(
        Guid userId,
        string? email,
        NotificationType type,
        string templateCode,
        Dictionary<string, string> placeholders,
        string? relatedEntityType,
        Guid? relatedEntityId,
        CancellationToken cancellationToken,
        bool emailOnly = false)
    {
        // ── In-App Notification ──────────────────────────────────
        if (!emailOnly)
        {
            var inAppTemplate = await _repository.GetActiveTemplateByCodeAsync(templateCode, NotificationChannel.InApp, cancellationToken);

            string title;
            string message;

            if (inAppTemplate != null)
            {
                title = RenderTemplate(inAppTemplate.SubjectTemplate, placeholders);
                message = RenderTemplate(inAppTemplate.BodyTemplate, placeholders);
            }
            else
            {
                // Graceful fallback: use type name as title, no crash
                title = type.ToString();
                message = $"You have a new notification: {type}";
            }

            var inAppNotification = new Notification
            {
                UserId = userId,
                Type = type,
                Channel = NotificationChannel.InApp,
                Title = title,
                Message = message,
                Status = NotificationStatus.Sent,
                SentAt = DateTime.UtcNow,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId
            };

            _repository.Add(inAppNotification);
        }

        // ── Email Notification ───────────────────────────────────
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailTemplate = await _repository.GetActiveTemplateByCodeAsync(templateCode, NotificationChannel.Email, cancellationToken);

            if (emailTemplate != null)
            {
                var subject = RenderTemplate(emailTemplate.SubjectTemplate, placeholders);
                var body = RenderTemplate(emailTemplate.BodyTemplate, placeholders);

                var emailNotification = new Notification
                {
                    UserId = userId,
                    Type = type,
                    Channel = NotificationChannel.Email,
                    Title = subject,
                    Message = body,
                    Status = NotificationStatus.Pending,
                    RelatedEntityType = relatedEntityType,
                    RelatedEntityId = relatedEntityId
                };

                _repository.Add(emailNotification);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Attempt dispatch synchronously
                await DispatchEmailAsync(emailNotification, email, subject, body, cancellationToken);
            }
            else
            {
                // Log failure but do not crash calling business logic
                var failedNotification = new Notification
                {
                    UserId = userId,
                    Type = type,
                    Channel = NotificationChannel.Email,
                    Title = type.ToString(),
                    Message = "Template not found or inactive.",
                    Status = NotificationStatus.Failed,
                    RelatedEntityType = relatedEntityType,
                    RelatedEntityId = relatedEntityId
                };
                _repository.Add(failedNotification);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEmailAsync(Notification notification, string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            var providerResponse = await _emailSender.SendEmailAsync(toEmail, subject, body, cancellationToken);

            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;

            _repository.AddDispatchLog(new NotificationDispatchLog
            {
                NotificationId = notification.Id,
                Channel = NotificationChannel.Email,
                Status = NotificationStatus.Sent,
                ProviderResponse = providerResponse,
                AttemptedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            notification.Status = NotificationStatus.Failed;

            _repository.AddDispatchLog(new NotificationDispatchLog
            {
                NotificationId = notification.Id,
                Channel = NotificationChannel.Email,
                Status = NotificationStatus.Failed,
                // Avoid logging sensitive content; keep only exception type and message
                ProviderResponse = $"{ex.GetType().Name}: {ex.Message}",
                AttemptedAt = DateTime.UtcNow
            });
        }

        _repository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string RenderTemplate(string template, Dictionary<string, string> placeholders)
    {
        var result = template;
        foreach (var kvp in placeholders)
        {
            result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }
        return result;
    }
}
