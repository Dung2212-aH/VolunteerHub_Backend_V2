using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Notification notification) => _context.Notifications.Add(notification);

    public void Update(Notification notification) => _context.Notifications.Update(notification);

    public void AddDispatchLog(NotificationDispatchLog log) => _context.NotificationDispatchLogs.Add(log);

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && n.Channel == NotificationChannel.InApp)
            .OrderByDescending(n => n.CreatedAt)
            .Take(100) // Safety cap to avoid unbounded queries
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId
                          && n.Channel == NotificationChannel.InApp
                          && n.Status != NotificationStatus.Read, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetActiveTemplateByCodeAsync(string code, NotificationChannel channel, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Code == code && t.Channel == channel && t.IsActive, cancellationToken);
    }
}
