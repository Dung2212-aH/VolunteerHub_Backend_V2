using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IOrganizerVerificationService
{
    Task<bool> IsOrganizerVerifiedAsync(Guid organizerId, CancellationToken cancellationToken = default);
    
    // Admin ops
    Task<Result<List<OrganizerProfileResponse>>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default);
    Task<Result> ApproveOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default);
    Task<Result> RejectOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default);
    Task<Result> SuspendOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default);
}
