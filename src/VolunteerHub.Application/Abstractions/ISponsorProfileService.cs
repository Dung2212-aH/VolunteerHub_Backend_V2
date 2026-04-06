using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface ISponsorProfileService
{
    Task<Result<SponsorProfileResponse>> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> CreateProfileAsync(Guid userId, CreateSponsorProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateProfileAsync(Guid userId, UpdateSponsorProfileRequest request, CancellationToken cancellationToken = default);

    Task<Result> RequestSponsorEventAsync(Guid userId, SponsorEventRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<EventSponsorResponse>>> GetMyEventSponsorsAsync(Guid userId, CancellationToken cancellationToken = default);
}