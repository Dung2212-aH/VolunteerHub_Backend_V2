using VolunteerHub.Application.Common;

namespace VolunteerHub.Application.Abstractions;

public interface ICertificateEligibilityService
{
    Task<Result> CheckEligibilityAsync(Guid volunteerProfileId, Guid eventId, CancellationToken cancellationToken = default);
}
