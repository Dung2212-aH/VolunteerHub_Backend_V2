using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface ISponsorManagementService
{
    Task<Result<List<SponsorProfileResponse>>> GetPendingSponsorProfilesAsync(CancellationToken cancellationToken = default);
    Task<Result> ReviewSponsorProfileAsync(Guid sponsorProfileId, ApproveSponsorProfileRequest request, CancellationToken cancellationToken = default);

    Task<Result<List<SponsorshipPackageResponse>>> GetEventPackagesAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default);
    Task<Result> CreatePackageAsync(Guid organizerUserId, Guid eventId, CreateSponsorshipPackageRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdatePackageAsync(Guid organizerUserId, Guid packageId, UpdateSponsorshipPackageRequest request, CancellationToken cancellationToken = default);

    Task<Result<List<EventSponsorResponse>>> GetEventSponsorsAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default);
    Task<Result> ReviewEventSponsorAsync(Guid organizerUserId, Guid eventSponsorId, ApproveRejectEventSponsorRequest request, CancellationToken cancellationToken = default);

    Task<Result> RecordContributionAsync(Guid organizerUserId, RecordContributionRequest request, CancellationToken cancellationToken = default);
}