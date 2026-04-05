using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class OrganizerProfileService : IOrganizerProfileService
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizerProfileService(IOrganizerRepository organizerRepository, IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrganizerProfileResponse>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _organizerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) return Result.Failure<OrganizerProfileResponse>(Error.NotFound);
        return Result.Success(MapToResponse(profile));
    }

    public async Task<Result<OrganizerProfileResponse>> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profile = await _organizerRepository.GetByIdAsync(profileId, cancellationToken);
        if (profile == null) return Result.Failure<OrganizerProfileResponse>(Error.NotFound);
        return Result.Success(MapToResponse(profile));
    }

    public async Task<Result> CreateProfileAsync(Guid userId, CreateOrganizerProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (await _organizerRepository.ExistsForUserAsync(userId, cancellationToken))
            return Result.Failure(new Error("Organizer.AlreadyExists", "Organizer profile already exists for this user."));

        var profile = new OrganizerProfile
        {
            UserId = userId,
            OrganizationName = request.OrganizationName,
            OrganizationType = request.OrganizationType,
            RegistrationNumber = request.RegistrationNumber,
            TaxCode = request.TaxCode,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Description = request.Description,
            Mission = request.Mission,
            WebsiteUrl = request.WebsiteUrl,
            LogoUrl = request.LogoUrl,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            VerificationStatus = OrganizerVerificationStatus.Pending
        };

        _organizerRepository.Add(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateProfileAsync(Guid userId, UpdateOrganizerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _organizerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) return Result.Failure(Error.NotFound);

        // Security / Business logic: If approved, legally bound items cannot change.
        if (profile.VerificationStatus == OrganizerVerificationStatus.Approved)
        {
            if (!string.Equals(profile.OrganizationName, request.OrganizationName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(profile.RegistrationNumber, request.RegistrationNumber, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(profile.TaxCode, request.TaxCode, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(new Error("Organizer.ImmutableLegals", "Legally verified fields (Name, Registration, TaxCode) cannot be modified after approval."));
            }
        }

        profile.OrganizationName = request.OrganizationName;
        profile.OrganizationType = request.OrganizationType;
        profile.RegistrationNumber = request.RegistrationNumber;
        profile.TaxCode = request.TaxCode;
        profile.Email = request.Email;
        profile.Phone = request.Phone;
        profile.Address = request.Address;
        profile.Description = request.Description;
        profile.Mission = request.Mission;
        profile.WebsiteUrl = request.WebsiteUrl;
        profile.LogoUrl = request.LogoUrl;
        profile.Latitude = request.Latitude;
        profile.Longitude = request.Longitude;

        _organizerRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UploadDocumentAsync(Guid userId, UploadLegalDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _organizerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) return Result.Failure(Error.NotFound);

        var doc = new OrganizerLegalDocument
        {
            OrganizerProfileId = profile.Id,
            DocumentType = request.DocumentType,
            FilePath = request.FilePath,
            OriginalFileName = request.OriginalFileName,
            Status = "Uploaded",
            UploadedAt = DateTime.UtcNow
        };

        profile.LegalDocuments.Add(doc);
        _organizerRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SubmitForVerificationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _organizerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null) return Result.Failure(Error.NotFound);

        if (profile.VerificationStatus == OrganizerVerificationStatus.Approved || profile.VerificationStatus == OrganizerVerificationStatus.UnderReview)
            return Result.Failure(new Error("Organizer.InvalidStatusTransition", "Profile is already approved or under review."));

        if (!profile.LegalDocuments.Any())
            return Result.Failure(new Error("Organizer.NoDocuments", "Cannot submit for verification without legal documents."));

        profile.VerificationStatus = OrganizerVerificationStatus.UnderReview;
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
