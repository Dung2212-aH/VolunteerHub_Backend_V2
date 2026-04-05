using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class ApplicationApprovalRepository : IApplicationApprovalRepository
{
    private readonly AppDbContext _context;

    public ApplicationApprovalRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddApplication(EventApplication application) => _context.EventApplications.Add(application);

    public void UpdateApplication(EventApplication application) => _context.EventApplications.Update(application);

    public async Task<EventApplication?> GetApplicationByIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.EventApplications
            .Include(a => a.ReviewNotes)
            .Include(a => a.DecisionHistory)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
    }

    public async Task<List<EventApplication>> GetApplicationsByEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.EventApplications
            .Include(a => a.VolunteerProfile)
            .Where(a => a.EventId == eventId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EventApplication>> GetMyApplicationsAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.EventApplications
            .Include(a => a.Event)
            .Where(a => a.VolunteerProfileId == volunteerProfileId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveApplicationAsync(Guid eventId, Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] 
        { 
            ApplicationStatus.Pending, 
            ApplicationStatus.UnderReview, 
            ApplicationStatus.Waitlisted, 
            ApplicationStatus.Approved 
        };

        return await _context.EventApplications
            .AnyAsync(a => a.EventId == eventId 
                        && a.VolunteerProfileId == volunteerProfileId 
                        && activeStatuses.Contains(a.Status), cancellationToken);
    }

    public async Task<bool> IsApprovedAsync(Guid eventId, Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.EventApplications
            .AnyAsync(a => a.EventId == eventId 
                        && a.VolunteerProfileId == volunteerProfileId 
                        && a.Status == ApplicationStatus.Approved, cancellationToken);
    }

    public async Task<int> GetApprovedApplicationsCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.EventApplications
            .CountAsync(a => a.EventId == eventId && a.Status == ApplicationStatus.Approved, cancellationToken);
    }

    public void AddReviewNote(ApplicationReviewNote note) => _context.ApplicationReviewNotes.Add(note);

    public void AddDecisionHistory(ApplicationDecisionHistory history) => _context.ApplicationDecisionHistories.Add(history);
}
