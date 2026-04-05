using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IRatingRepository
{
    void Add(Rating rating);
    Task<Rating?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid eventId, Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);
    Task<List<Rating>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Rating>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<List<Rating>> GetRatingsForUserAsync(Guid targetUserId, CancellationToken cancellationToken = default);

    void AddFeedbackReport(FeedbackReport report);
    Task<List<FeedbackReport>> GetFeedbackByReporterAsync(Guid reporterUserId, CancellationToken cancellationToken = default);
}
