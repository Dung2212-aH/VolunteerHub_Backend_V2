using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class SponsorManagementService : ISponsorManagementService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdminAuditService _adminAuditService;

    public SponsorManagementService(
    ISponsorRepository sponsorRepository,
    IUnitOfWork unitOfWork,
    IAdminAuditService adminAuditService)
    {
        _sponsorRepository = sponsorRepository;
        _unitOfWork = unitOfWork;
        _adminAuditService = adminAuditService;
    }

    public async Task<Result<List<SponsorProfileResponse>>> GetPendingSponsorProfilesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _sponsorRepository.GetPendingSponsorProfilesAsync(cancellationToken);
        return Result.Success(items.Select(MapToSponsorProfileResponse).ToList());
    }

    public async Task<Result> ReviewSponsorProfileAsync(Guid sponsorProfileId, ApproveSponsorProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _sponsorRepository.GetSponsorProfileByIdAsync(sponsorProfileId, cancellationToken);
        if (profile == null)
            return Result.Failure(Error.NotFound);

        profile.Status = request.Approve ? SponsorProfileStatus.Approved : SponsorProfileStatus.Rejected;
        profile.RejectionReason = request.Approve ? null : request.Reason;

        _sponsorRepository.UpdateSponsorProfile(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _adminAuditService.LogAsync(
            Guid.Empty,
            request.Approve ? "SponsorProfile.Approved" : "SponsorProfile.Rejected",
            nameof(SponsorProfile),
            profile.Id,
            request.Reason ?? string.Empty,
            cancellationToken);
        return Result.Success();
    }

    public async Task<Result<List<SponsorshipPackageResponse>>> GetEventPackagesAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _sponsorRepository.GetEventByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerUserId)
            return Result.Failure<List<SponsorshipPackageResponse>>(Error.NotFound);

        var items = await _sponsorRepository.GetPackagesByEventIdAsync(eventId, cancellationToken);
        return Result.Success(items.Select(MapToPackageResponse).ToList());
    }

    public async Task<Result> CreatePackageAsync(Guid organizerUserId, Guid eventId, CreateSponsorshipPackageRequest request, CancellationToken cancellationToken = default)
    {
        var ev = await _sponsorRepository.GetEventByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerUserId)
            return Result.Failure(Error.NotFound);

        var package = new SponsorshipPackage
        {
            EventId = eventId,
            Name = request.Name,
            Description = request.Description,
            Amount = request.Amount,
            Benefits = request.Benefits,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        _sponsorRepository.AddSponsorshipPackage(package);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdatePackageAsync(Guid organizerUserId, Guid packageId, UpdateSponsorshipPackageRequest request, CancellationToken cancellationToken = default)
    {
        var package = await _sponsorRepository.GetPackageByIdAsync(packageId, cancellationToken);
        if (package == null)
            return Result.Failure(Error.NotFound);

        var ev = await _sponsorRepository.GetEventByIdAsync(package.EventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerUserId)
            return Result.Failure(Error.NotFound);

        package.Name = request.Name;
        package.Description = request.Description;
        package.Amount = request.Amount;
        package.Benefits = request.Benefits;
        package.DisplayOrder = request.DisplayOrder;
        package.IsActive = request.IsActive;

        _sponsorRepository.UpdateSponsorshipPackage(package);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<EventSponsorResponse>>> GetEventSponsorsAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _sponsorRepository.GetEventByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerUserId)
            return Result.Failure<List<EventSponsorResponse>>(Error.NotFound);

        var items = await _sponsorRepository.GetEventSponsorsByEventIdAsync(eventId, cancellationToken);
        return Result.Success(items.Select(MapToEventSponsorResponse).ToList());
    }

    public async Task<Result> ReviewEventSponsorAsync(Guid organizerUserId, Guid eventSponsorId, ApproveRejectEventSponsorRequest request, CancellationToken cancellationToken = default)
    {
        var eventSponsor = await _sponsorRepository.GetEventSponsorByIdAsync(eventSponsorId, cancellationToken);
        if (eventSponsor == null || eventSponsor.Event.OrganizerId != organizerUserId)
            return Result.Failure(Error.NotFound);

        eventSponsor.Status = request.Approve ? EventSponsorStatus.Approved : EventSponsorStatus.Rejected;
        eventSponsor.RejectionReason = request.Approve ? null : request.Reason;
        eventSponsor.IsPubliclyVisible = request.Approve && request.IsPubliclyVisible;
        eventSponsor.DisplayPriority = request.DisplayPriority;

        _sponsorRepository.UpdateEventSponsor(eventSponsor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RecordContributionAsync(Guid organizerUserId, RecordContributionRequest request, CancellationToken cancellationToken = default)
    {
        var eventSponsor = await _sponsorRepository.GetEventSponsorByIdAsync(request.EventSponsorId, cancellationToken);
        if (eventSponsor == null || eventSponsor.Event.OrganizerId != organizerUserId)
            return Result.Failure(Error.NotFound);

        if (eventSponsor.Status != EventSponsorStatus.Approved)
            return Result.Failure(new Error("Sponsor.InvalidSponsorStatus", "Only approved event sponsors can have contributions recorded."));

        if (!Enum.TryParse<ContributionType>(request.Type, true, out var contributionType))
            return Result.Failure(new Error("Sponsor.InvalidContributionType", "Contribution type must be Monetary or InKind."));

        var contribution = new SponsorContribution
        {
            EventSponsorId = eventSponsor.Id,
            SponsorProfileId = eventSponsor.SponsorProfileId,
            Type = contributionType,
            Value = request.Value,
            Description = request.Description,
            ContributedAt = request.ContributedAt ?? DateTime.UtcNow,
            ReceiptReference = request.ReceiptReference,
            Note = request.Note
        };

        _sponsorRepository.AddContribution(contribution);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

    private static SponsorshipPackageResponse MapToPackageResponse(SponsorshipPackage package)
    {
        return new SponsorshipPackageResponse
        {
            Id = package.Id,
            EventId = package.EventId,
            Name = package.Name,
            Description = package.Description,
            Amount = package.Amount,
            Benefits = package.Benefits,
            DisplayOrder = package.DisplayOrder,
            IsActive = package.IsActive
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