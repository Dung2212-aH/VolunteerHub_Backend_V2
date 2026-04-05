using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IVolunteerProfileRepository
{
    Task<VolunteerProfile?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<VolunteerProfile?> GetByIdWithDetailsAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(VolunteerProfile profile);
    void Update(VolunteerProfile profile);
}
