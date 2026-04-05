using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class ApplicationReviewService : IApplicationReviewService
{
    private readonly IApplicationApprovalRepository _appRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationReviewService(
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

    private async Task<Result<(EventApplication Application, Event Event)>> GetValidApplicationAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await _appRepository.GetApplicationByIdAsync(applicationId, cancellationToken);
        if (application == null) return Result.Failure<(EventApplication, Event)>(Error.NotFound);

        var ev = await _eventRepository.GetDetailsByIdAsync(application.EventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId) 
            return Result.Failure<(EventApplication, Event)>(Error.Unauthorized);

        return Result.Success((application, ev));
    }

    private bool IsTransitionAllowed(ApplicationStatus current, ApplicationStatus target)
    {
        return (current, target) switch
        {
            (ApplicationStatus.Pending, ApplicationStatus.UnderReview) => true,
            (ApplicationStatus.Pending, ApplicationStatus.Approved) => true,
            (ApplicationStatus.Pending, ApplicationStatus.Rejected) => true,
            (ApplicationStatus.Pending, ApplicationStatus.Waitlisted) => true,
            
            (ApplicationStatus.UnderReview, ApplicationStatus.Approved) => true,
            (ApplicationStatus.UnderReview, ApplicationStatus.Rejected) => true,
            (ApplicationStatus.UnderReview, ApplicationStatus.Waitlisted) => true,
            
            (ApplicationStatus.Waitlisted, ApplicationStatus.Approved) => true,
            (ApplicationStatus.Waitlisted, ApplicationStatus.Rejected) => true,
            
            (ApplicationStatus.Approved, ApplicationStatus.Cancelled) => true,
            
            _ => false
        };
    }

    private async Task<Result> ChangeStatusAsync(Guid organizerId, Guid applicationId, ApplicationStatus targetStatus, string reason, CancellationToken cancellationToken)
    {
        var appResult = await GetValidApplicationAsync(organizerId, applicationId, cancellationToken);
        if (!appResult.IsSuccess) return Result.Failure(appResult.Error);

        var application = appResult.Value.Application;
        var ev = appResult.Value.Event;

        if (!IsTransitionAllowed(application.Status, targetStatus))
        {
            return Result.Failure(new Error("Application.InvalidTransition", $"Cannot transition from {application.Status} to {targetStatus}."));
        }

        // Rule: Verify Event Capacity on Approval
        if (targetStatus == ApplicationStatus.Approved)
        {
            var approvedCount = await _appRepository.GetApprovedApplicationsCountAsync(application.EventId, cancellationToken);
            if (approvedCount >= ev.Capacity)
            {
                return Result.Failure(new Error("Application.CapacityExceeded", "The event capacity has been reached. You cannot approve further applications."));
            }
        }

        var prevStatus = application.Status;
        application.Status = targetStatus;
        application.ReviewedAt = DateTime.UtcNow;
        application.ReviewedByUserId = organizerId;

        if (targetStatus == ApplicationStatus.Rejected && !string.IsNullOrWhiteSpace(reason))
        {
            application.RejectionReason = reason;
        }

        _appRepository.UpdateApplication(application);
        _appRepository.AddDecisionHistory(new ApplicationDecisionHistory
        {
            EventApplicationId = application.Id,
            PreviousStatus = prevStatus,
            NewStatus = targetStatus,
            ChangedByUserId = organizerId,
            Reason = string.IsNullOrWhiteSpace(reason) ? $"Organizer Status Change to {targetStatus}" : reason
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Trigger notifications for approval/rejection — resolve profileId to Identity userId
        var volunteerProfile = await _profileRepository.GetByIdWithDetailsAsync(application.VolunteerProfileId, cancellationToken);
        if (volunteerProfile != null)
        {
            if (targetStatus == ApplicationStatus.Approved)
            {
                await _notificationService.NotifyApplicationApprovedAsync(
                    volunteerProfile.UserId, ev.Title, application.Id, cancellationToken);
            }
            else if (targetStatus == ApplicationStatus.Rejected)
            {
                await _notificationService.NotifyApplicationRejectedAsync(
                    volunteerProfile.UserId, ev.Title, application.Id, reason, cancellationToken);
            }
        }

        return Result.Success();
    }

    public Task<Result> ApproveAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default)
        => ChangeStatusAsync(organizerId, applicationId, ApplicationStatus.Approved, string.Empty, cancellationToken);

    public Task<Result> RejectAsync(Guid organizerId, Guid applicationId, ReviewApplicationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Task.FromResult(Result.Failure(new Error("Application.ReasonRequired", "A reason is required to reject an application.")));

        return ChangeStatusAsync(organizerId, applicationId, ApplicationStatus.Rejected, request.Reason, cancellationToken);
    }

    public Task<Result> WaitlistAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default)
        => ChangeStatusAsync(organizerId, applicationId, ApplicationStatus.Waitlisted, string.Empty, cancellationToken);

    public async Task<Result> AddNoteAsync(Guid organizerId, Guid applicationId, AddReviewNoteRequest request, CancellationToken cancellationToken = default)
    {
        var appResult = await GetValidApplicationAsync(organizerId, applicationId, cancellationToken);
        if (!appResult.IsSuccess) return Result.Failure(appResult.Error);

        var note = new ApplicationReviewNote
        {
            EventApplicationId = applicationId,
            AuthorUserId = organizerId,
            Content = request.Content,
            IsPrivate = request.IsPrivate
        };

        _appRepository.AddReviewNote(note);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<ApplicantSummaryResponse>>> GetEventApplicationsAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId) return Result.Failure<List<ApplicantSummaryResponse>>(Error.Unauthorized);

        var apps = await _appRepository.GetApplicationsByEventAsync(eventId, cancellationToken);
        return Result.Success(apps.Select(app => new ApplicantSummaryResponse
        {
            Id = app.Id,
            EventId = app.EventId,
            VolunteerProfileId = app.VolunteerProfileId,
            Status = app.Status.ToString(),
            AppliedAt = app.AppliedAt,
            MotivationText = app.MotivationText,
            AvailabilityNote = app.AvailabilityNote,
            ReviewedAt = app.ReviewedAt,
            RejectionReason = app.RejectionReason,
            VolunteerFullName = "Participant Data"
        }).ToList());
    }

    public async Task<Result<ApplicantSummaryResponse>> GetApplicationDetailsAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var appResult = await GetValidApplicationAsync(organizerId, applicationId, cancellationToken);
        if (!appResult.IsSuccess) return Result.Failure<ApplicantSummaryResponse>(appResult.Error);

        var app = appResult.Value.Application;
        return Result.Success(new ApplicantSummaryResponse
        {
            Id = app.Id,
            EventId = app.EventId,
            VolunteerProfileId = app.VolunteerProfileId,
            Status = app.Status.ToString(),
            AppliedAt = app.AppliedAt,
            MotivationText = app.MotivationText,
            AvailabilityNote = app.AvailabilityNote,
            ReviewedAt = app.ReviewedAt,
            RejectionReason = app.RejectionReason,
            VolunteerFullName = "Participant Data"
        });
    }
}
