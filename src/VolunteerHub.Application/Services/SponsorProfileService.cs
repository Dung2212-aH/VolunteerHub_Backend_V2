using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class SponsorProfileService : ISponsorProfileService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SponsorProfileService(ISponsorRepository sponsorRepository, IUnitOfWork unitOfWork)
    {
        _sponsorRepository = sponsorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SponsorProfileResponse>> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _sponsorRepository.GetSponsorProfileByUserIdAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure<SponsorProfileResponse>(Error.NotFound);

        return Result.Success(MapToSponsorProfileResponse(profile));
    }

    public async Task<Result> CreateProfileAsync(Guid userId, CreateSponsorProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (await _sponsorRepository.SponsorProfileExistsForUserAsync(userId, cancellationToken))
            return Result.Failure(new Error("Sponsor.AlreadyExists", "Sponsor profile already exists for this user."));

        var profile = new SponsorProfile
        {
            UserId = userId,
            CompanyName = request.CompanyName,
            Description = request.Description,
            LogoUrl = request.LogoUrl ?? string.Empty,
            WebsiteUrl = request.WebsiteUrl ?? string.Empty,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            TaxCode = request.TaxCode,
            Status = SponsorProfileStatus.PendingApproval
        };

        foreach (var contact in request.ContactPersons)
        {
            profile.ContactPersons.Add(new SponsorContactPerson
            {
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Role = contact.Role
            });
        }

        _sponsorRepository.AddSponsorProfile(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateProfileAsync(Guid userId, UpdateSponsorProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _sponsorRepository.GetSponsorProfileByUserIdAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure(Error.NotFound);

        if (profile.Status == SponsorProfileStatus.Suspended)
            return Result.Failure(new Error("Sponsor.Suspended", "Suspended sponsor profiles cannot be updated."));

        profile.CompanyName = request.CompanyName;
        profile.Description = request.Description;
        profile.LogoUrl = request.LogoUrl ?? string.Empty;
        profile.WebsiteUrl = request.WebsiteUrl ?? string.Empty;
        profile.Email = request.Email;
        profile.Phone = request.Phone;
        profile.Address = request.Address;
        profile.TaxCode = request.TaxCode;

        profile.ContactPersons.Clear();
        foreach (var contact in request.ContactPersons)
        {
            profile.ContactPersons.Add(new SponsorContactPerson
            {
                SponsorProfileId = profile.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Role = contact.Role
            });
        }

        if (profile.Status == SponsorProfileStatus.Rejected)
        {
            profile.Status = SponsorProfileStatus.PendingApproval;
            profile.RejectionReason = null;
        }

        _sponsorRepository.UpdateSponsorProfile(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RequestSponsorEventAsync(Guid userId, SponsorEventRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _sponsorRepository.GetSponsorProfileByUserIdAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure(Error.NotFound);

        if (profile.Status != SponsorProfileStatus.Approved)
            return Result.Failure(new Error("Sponsor.ProfileNotApproved", "Only approved sponsor profiles can request event sponsorship."));

        var ev = await _sponsorRepository.GetEventByIdAsync(request.EventId, cancellationToken);
        if (ev == null)
            return Result.Failure(Error.NotFound);

        SponsorshipPackage? package = null;
        if (request.SponsorshipPackageId.HasValue)
        {
            package = await _sponsorRepository.GetPackageByIdAsync(request.SponsorshipPackageId.Value, cancellationToken);
            if (package == null || package.EventId != request.EventId)
                return Result.Failure(new Error("Sponsor.PackageNotFound", "Sponsorship package not found for this event."));

            if (!package.IsActive)
                return Result.Failure(new Error("Sponsor.PackageInactive", "Selected sponsorship package is inactive."));
        }

        var alreadyExists = await _sponsorRepository.HasActiveEventSponsorAsync(profile.Id, request.EventId, cancellationToken);
        if (alreadyExists)
            return Result.Failure(new Error("Sponsor.DuplicateRequest", "This sponsor already has an active sponsorship request for the event."));

        var eventSponsor = new EventSponsor
        {
            SponsorProfileId = profile.Id,
            EventId = request.EventId,
            SponsorshipPackageId = request.SponsorshipPackageId,
            Status = EventSponsorStatus.Pending,
            DisplayPriority = 0,
            IsPubliclyVisible = false,
            Note = request.Note
        };

        _sponsorRepository.AddEventSponsor(eventSponsor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<EventSponsorResponse>>> GetMyEventSponsorsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _sponsorRepository.GetSponsorProfileByUserIdAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure<List<EventSponsorResponse>>(Error.NotFound);

        var items = await _sponsorRepository.GetEventSponsorsBySponsorProfileIdAsync(profile.Id, cancellationToken);
        return Result.Success(items.Select(MapToEventSponsorResponse).ToList());
    }

    private static SponsorProfileResponse MapToSponsorProfileResponse(SponsorProfile profile)
    {
        return new SponsorProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            CompanyName = profile.CompanyName,
            Description = profile.Description,
            LogoUrl = profile.LogoUrl,
            WebsiteUrl = profile.WebsiteUrl,
            Email = profile.Email,
            Phone = profile.Phone,
            Address = profile.Address,
            TaxCode = profile.TaxCode,
            Status = profile.Status.ToString(),
            RejectionReason = profile.RejectionReason,
            ContactPersons = profile.ContactPersons.Select(x => new ContactPersonResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role
            }).ToList()
        };
    }

    private static EventSponsorResponse MapToEventSponsorResponse(EventSponsor item)
    {
        return new EventSponsorResponse
        {
            Id = item.Id,
            SponsorProfileId = item.SponsorProfileId,
            CompanyName = item.SponsorProfile.CompanyName,
            LogoUrl = item.SponsorProfile.LogoUrl,
            EventId = item.EventId,
            EventTitle = item.Event.Title,
            SponsorshipPackageId = item.SponsorshipPackageId,
            PackageName = item.SponsorshipPackage?.Name,
            Status = item.Status.ToString(),
            DisplayPriority = item.DisplayPriority,
            IsPubliclyVisible = item.IsPubliclyVisible,
            Note = item.Note,
            RejectionReason = item.RejectionReason
        };
    }
}