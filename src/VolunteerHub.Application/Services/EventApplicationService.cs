using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class EventApplicationService : IEventApplicationService
{
    private readonly IApplicationApprovalRepository _appRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public EventApplicationService(
        IApplicationApprovalRepository appRepository,
        IEventRepository eventRepository,
        IVolunteerProfileRepository profileRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _appRepository = appRepository;
        _eventRepository = eventRepository;
        _profileRepository = profileRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ApplyAsync(Guid volunteerProfileId, ApplyToEventRequest request, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(request.EventId, cancellationToken);
        if (ev == null) return Result.Failure(Error.NotFound);

        // Cannot apply if not actively published
        if (ev.Status != EventStatus.Published) 
            return Result.Failure(new Error("Application.EventNotOpen", "You cannot apply to an event that is not dynamically published."));

        // Enforce single active application rule
        var hasActive = await _appRepository.HasActiveApplicationAsync(request.EventId, volunteerProfileId, cancellationToken);
        if (hasActive) 
            return Result.Failure(new Error("Application.Duplicate", "You already have an active application for this event."));

        var application = new EventApplication
        {
            EventId = request.EventId,
            VolunteerProfileId = volunteerProfileId,
            MotivationText = request.MotivationText,
            AvailabilityNote = request.AvailabilityNote,
            Status = ApplicationStatus.Pending
        };

        var history = new ApplicationDecisionHistory
        {
            EventApplication = application,
            PreviousStatus = ApplicationStatus.Pending,
            NewStatus = ApplicationStatus.Pending,
            ChangedByUserId = volunteerProfileId,
            Reason = "Initial Application"
        };

        _appRepository.AddApplication(application);
        _appRepository.AddDecisionHistory(history);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Trigger notification — resolve profileId to Identity userId
        var profile = await _profileRepository.GetByIdWithDetailsAsync(volunteerProfileId, cancellationToken);
        if (profile != null)
        {
            await _notificationService.NotifyApplicationSubmittedAsync(
                profile.UserId, ev.Title, application.Id, cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> WithdrawAsync(Guid volunteerProfileId, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var application = await _appRepository.GetApplicationByIdAsync(applicationId, cancellationToken);
        if (application == null || application.VolunteerProfileId != volunteerProfileId) 
            return Result.Failure(Error.NotFound);

        if (application.Status != ApplicationStatus.Pending && 
            application.Status != ApplicationStatus.UnderReview && 
            application.Status != ApplicationStatus.Waitlisted)
        {
            return Result.Failure(new Error("Application.InvalidTransition", "Cannot withdraw an application from this state."));
        }

        var prevStatus = application.Status;
        application.Status = ApplicationStatus.Withdrawn;
        application.WithdrawnAt = DateTime.UtcNow;

        _appRepository.UpdateApplication(application);
        _appRepository.AddDecisionHistory(new ApplicationDecisionHistory
        {
            EventApplicationId = application.Id,
            PreviousStatus = prevStatus,
            NewStatus = ApplicationStatus.Withdrawn,
            ChangedByUserId = volunteerProfileId,
            Reason = "Voluntary Withdrawal"
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<ApplicationResponse>> GetApplicationAsync(Guid volunteerProfileId, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var app = await _appRepository.GetApplicationByIdAsync(applicationId, cancellationToken);
        if (app == null || app.VolunteerProfileId != volunteerProfileId) return Result.Failure<ApplicationResponse>(Error.NotFound);

        return Result.Success(new ApplicationResponse
        {
            Id = app.Id,
            EventId = app.EventId,
            VolunteerProfileId = app.VolunteerProfileId,
            Status = app.Status.ToString(),
            AppliedAt = app.AppliedAt,
            MotivationText = app.MotivationText,
            AvailabilityNote = app.AvailabilityNote,
            ReviewedAt = app.ReviewedAt,
            RejectionReason = app.RejectionReason
        });
    }

    public async Task<Result<List<ApplicationResponse>>> GetMyApplicationsAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var apps = await _appRepository.GetMyApplicationsAsync(volunteerProfileId, cancellationToken);
        return Result.Success(apps.Select(app => new ApplicationResponse
        {
            Id = app.Id,
            EventId = app.EventId,
            VolunteerProfileId = app.VolunteerProfileId,
            Status = app.Status.ToString(),
            AppliedAt = app.AppliedAt,
            MotivationText = app.MotivationText,
            AvailabilityNote = app.AvailabilityNote,
            ReviewedAt = app.ReviewedAt,
            RejectionReason = app.RejectionReason
        }).ToList());
    }
}
