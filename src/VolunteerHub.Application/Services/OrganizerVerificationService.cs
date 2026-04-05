using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class OrganizerVerificationService : IOrganizerVerificationService
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizerVerificationService(IOrganizerRepository organizerRepository, IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsOrganizerVerifiedAsync(Guid organizerId, CancellationToken cancellationToken = default)
    {
        // organizerId correlates to UserId in Event tracking
        var profile = await _organizerRepository.GetByUserIdAsync(organizerId, cancellationToken);
        return profile != null && profile.VerificationStatus == OrganizerVerificationStatus.Approved;
    }

    public async Task<Result<List<OrganizerProfileResponse>>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default)
    {
        var pendingProfiles = await _organizerRepository.GetPendingProfilesAsync(cancellationToken);
        return Result.Success(pendingProfiles.Select(MapToResponse).ToList());
    }

    public async Task<Result> ApproveOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default)
    {
        return await ChangeStatusAsync(adminId, profileId, OrganizerVerificationStatus.Approved, request.Comment, cancellationToken);
    }

    public async Task<Result> RejectOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default)
    {
        return await ChangeStatusAsync(adminId, profileId, OrganizerVerificationStatus.Rejected, request.Comment, cancellationToken);
    }

    public async Task<Result> SuspendOrganizerAsync(Guid adminId, Guid profileId, ReviewOrganizerVerificationRequest request, CancellationToken cancellationToken = default)
    {
        return await ChangeStatusAsync(adminId, profileId, OrganizerVerificationStatus.Suspended, request.Comment, cancellationToken);
    }

    private async Task<Result> ChangeStatusAsync(Guid adminId, Guid profileId, OrganizerVerificationStatus newStatus, string comment, CancellationToken cancellationToken)
    {
        var profile = await _organizerRepository.GetByIdAsync(profileId, cancellationToken);
        if (profile == null) return Result.Failure(Error.NotFound);

        var review = new OrganizerVerificationReview
        {
            OrganizerProfileId = profile.Id,
            PreviousStatus = profile.VerificationStatus,
            NewStatus = newStatus,
            ReviewerId = adminId,
            Comment = comment,
            ReviewedAt = DateTime.UtcNow
        };

        if (newStatus == OrganizerVerificationStatus.Approved)
        {
            profile.VerifiedAt = DateTime.UtcNow;
            profile.RejectionReason = string.Empty;
        }
        else if (newStatus == OrganizerVerificationStatus.Rejected || newStatus == OrganizerVerificationStatus.Suspended)
        {
            profile.VerifiedAt = null;
            profile.RejectionReason = comment;
        }

        profile.VerificationStatus = newStatus;

        _organizerRepository.AddReview(review);
        _organizerRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private OrganizerProfileResponse MapToResponse(OrganizerProfile profile)
    {
        return new OrganizerProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            OrganizationName = profile.OrganizationName,
            OrganizationType = profile.OrganizationType,
            RegistrationNumber = profile.RegistrationNumber,
            TaxCode = profile.TaxCode,
            Email = profile.Email,
            Phone = profile.Phone,
            Address = profile.Address,
            Description = profile.Description,
            Mission = profile.Mission,
            WebsiteUrl = profile.WebsiteUrl,
            LogoUrl = profile.LogoUrl,
            Latitude = profile.Latitude,
            Longitude = profile.Longitude,
            VerificationStatus = profile.VerificationStatus.ToString(),
            VerifiedAt = profile.VerifiedAt,
            RejectionReason = profile.RejectionReason,
            LegalDocuments = profile.LegalDocuments.Select(d => new LegalDocumentResponse
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                FilePath = d.FilePath,
                OriginalFileName = d.OriginalFileName,
                Status = d.Status,
                UploadedAt = d.UploadedAt
            }).ToList()
        };
    }
}
