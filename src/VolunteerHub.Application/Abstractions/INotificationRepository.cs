using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Application.Abstractions;

public interface INotificationRepository
{
    void Add(Notification notification);
    void Update(Notification notification);
    void AddDispatchLog(NotificationDispatchLog log);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetActiveTemplateByCodeAsync(string code, NotificationChannel channel, CancellationToken cancellationToken = default);
}
