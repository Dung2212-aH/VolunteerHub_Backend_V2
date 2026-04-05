using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IOrganizerRepository
{
    Task<OrganizerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrganizerProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<OrganizerProfile>> GetPendingProfilesAsync(CancellationToken cancellationToken = default);
    
    void Add(OrganizerProfile profile);
    void Update(OrganizerProfile profile);
    
    void AddReview(OrganizerVerificationReview review);
}
