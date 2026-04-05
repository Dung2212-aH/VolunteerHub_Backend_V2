using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IOrganizerProfileService
{
    Task<Result<OrganizerProfileResponse>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<OrganizerProfileResponse>> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<Result> CreateProfileAsync(Guid userId, CreateOrganizerProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateProfileAsync(Guid userId, UpdateOrganizerProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> UploadDocumentAsync(Guid userId, UploadLegalDocumentRequest request, CancellationToken cancellationToken = default);
    Task<Result> SubmitForVerificationAsync(Guid userId, CancellationToken cancellationToken = default);
}
