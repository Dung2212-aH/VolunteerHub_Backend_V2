using VolunteerHub.Contracts.Recognition;
using VolunteerHub.Application.Common;

namespace VolunteerHub.Application.Abstractions;

public interface IBadgeService
{
    Task<Result> EvaluateBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
    Task<Result<List<BadgeResponse>>> GetMyBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
}
