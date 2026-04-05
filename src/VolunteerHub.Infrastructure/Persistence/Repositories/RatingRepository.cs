using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly AppDbContext _context;

    public RatingRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Rating rating) => _context.Ratings.Add(rating);

    public async Task<Rating?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid eventId, Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .AnyAsync(r => r.EventId == eventId && r.FromUserId == fromUserId && r.ToUserId == toUserId, cancellationToken);
    }

    public async Task<List<Rating>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .Where(r => r.FromUserId == userId || r.ToUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Rating>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Rating>> GetRatingsForUserAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Ratings
            .Where(r => r.ToUserId == targetUserId)
            .ToListAsync(cancellationToken);
    }

    public void AddFeedbackReport(FeedbackReport report) => _context.FeedbackReports.Add(report);

    public async Task<List<FeedbackReport>> GetFeedbackByReporterAsync(Guid reporterUserId, CancellationToken cancellationToken = default)
    {
        return await _context.FeedbackReports
            .Where(r => r.ReporterUserId == reporterUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
