using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Rating;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FeedbackService(
        IRatingRepository ratingRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _ratingRepository = ratingRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> SubmitReportAsync(Guid reporterUserId, CreateFeedbackReportRequest request, CancellationToken cancellationToken = default)
    {
        // Reason is required
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure(new Error("Feedback.ReasonRequired", "A reason is required for the report."));

        if (request.Reason.Length > 500)
            return Result.Failure(new Error("Feedback.ReasonTooLong", "Reason must not exceed 500 characters."));

        if (request.Description != null && request.Description.Length > 4000)
            return Result.Failure(new Error("Feedback.DescriptionTooLong", "Description must not exceed 4000 characters."));

        // Cannot report self
        if (reporterUserId == request.TargetUserId)
            return Result.Failure(new Error("Feedback.SelfReport", "You cannot report yourself."));

        // Validate event exists
        var ev = await _eventRepository.GetDetailsByIdAsync(request.EventId, cancellationToken);
        if (ev == null)
            return Result.Failure(Error.NotFound);

        // Validate that reporter and target belong to same event context:
        // reporter must be the organizer OR the target must be the organizer
        bool reporterIsOrganizer = ev.OrganizerId == reporterUserId;
        bool targetIsOrganizer = ev.OrganizerId == request.TargetUserId;

        if (!reporterIsOrganizer && !targetIsOrganizer)
            return Result.Failure(new Error("Feedback.InvalidContext", "The report must involve the event organizer."));

        var report = new FeedbackReport
        {
            EventId = request.EventId,
            ReporterUserId = reporterUserId,
            TargetUserId = request.TargetUserId,
            Reason = request.Reason,
            Description = request.Description
        };

        _ratingRepository.AddFeedbackReport(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<FeedbackReportResponse>>> GetMyReportsAsync(Guid reporterUserId, CancellationToken cancellationToken = default)
    {
        var reports = await _ratingRepository.GetFeedbackByReporterAsync(reporterUserId, cancellationToken);
        var response = reports.Select(r => new FeedbackReportResponse
        {
            Id = r.Id,
            EventId = r.EventId,
            ReporterUserId = r.ReporterUserId,
            TargetUserId = r.TargetUserId,
            Reason = r.Reason,
            Description = r.Description,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            ResolvedAt = r.ResolvedAt
        }).ToList();

        return Result.Success(response);
    }
}
