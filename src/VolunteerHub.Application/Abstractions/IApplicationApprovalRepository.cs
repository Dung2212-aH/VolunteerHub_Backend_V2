using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IApplicationApprovalRepository
{
    void AddApplication(EventApplication application);
    void UpdateApplication(EventApplication application);
    Task<EventApplication?> GetApplicationByIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<List<EventApplication>> GetApplicationsByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<List<EventApplication>> GetMyApplicationsAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
    
    Task<bool> HasActiveApplicationAsync(Guid eventId, Guid volunteerProfileId, CancellationToken cancellationToken = default);
    Task<bool> IsApprovedAsync(Guid eventId, Guid volunteerProfileId, CancellationToken cancellationToken = default);
    Task<int> GetApprovedApplicationsCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    
    void AddReviewNote(ApplicationReviewNote note);
    void AddDecisionHistory(ApplicationDecisionHistory history);
}
