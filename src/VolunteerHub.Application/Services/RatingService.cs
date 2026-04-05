using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Rating;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IApplicationApprovalRepository _appRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RatingService(
        IRatingRepository ratingRepository,
        IEventRepository eventRepository,
        IApplicationApprovalRepository appRepository,
        IAttendanceRepository attendanceRepository,
        IVolunteerProfileRepository profileRepository,
        IUnitOfWork unitOfWork)
    {
        _ratingRepository = ratingRepository;
        _eventRepository = eventRepository;
        _appRepository = appRepository;
        _attendanceRepository = attendanceRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Volunteer rates the organizer of an event.
    /// </summary>
    public async Task<Result> SubmitVolunteerRatingAsync(Guid volunteerUserId, CreateRatingRequest request, CancellationToken cancellationToken = default)
    {
        // Validate score
        if (request.Score < 1 || request.Score > 5)
            return Result.Failure(new Error("Rating.InvalidScore", "Score must be between 1 and 5."));

        // Validate comment length
        if (request.Comment != null && request.Comment.Length > 2000)
            return Result.Failure(new Error("Rating.CommentTooLong", "Comment must not exceed 2000 characters."));

        // Cannot rate self
        if (volunteerUserId == request.TargetUserId)
            return Result.Failure(new Error("Rating.SelfRating", "You cannot rate yourself."));

        // Get event and check status
        var ev = await _eventRepository.GetDetailsByIdAsync(request.EventId, cancellationToken);
        if (ev == null)
            return Result.Failure(Error.NotFound);
        if (ev.Status != EventStatus.Completed)
            return Result.Failure(new Error("Rating.EventNotCompleted", "You can only rate after the event is completed."));

        // The target must be the organizer of the event
        if (request.TargetUserId != ev.OrganizerId)
            return Result.Failure(new Error("Rating.InvalidTarget", "The target must be the organizer of this event."));

        // Resolve volunteer profile from userId (approval/attendance repos use profileId)
        var volunteerProfile = await _profileRepository.GetByUserIdWithDetailsAsync(volunteerUserId, cancellationToken);
        if (volunteerProfile == null)
            return Result.Failure(new Error("Rating.NoProfile", "Volunteer profile not found."));

        // Validate participation: approved application
        var isApproved = await _appRepository.IsApprovedAsync(request.EventId, volunteerProfile.Id, cancellationToken);
        if (!isApproved)
            return Result.Failure(new Error("Rating.NoApprovedApplication", "You must have an approved application to rate this event."));

        // Validate participation: approved attendance
        var hasAttendance = await _attendanceRepository.HasApprovedAttendanceAsync(request.EventId, volunteerProfile.Id, cancellationToken);
        if (!hasAttendance)
            return Result.Failure(new Error("Rating.NoApprovedAttendance", "You must have approved attendance to rate this event."));

        // Prevent duplicate rating
        var exists = await _ratingRepository.ExistsAsync(request.EventId, volunteerUserId, request.TargetUserId, cancellationToken);
        if (exists)
            return Result.Failure(new Error("Rating.Duplicate", "You have already rated this organizer for this event."));

        var rating = new Rating
        {
            EventId = request.EventId,
            FromUserId = volunteerUserId,
            ToUserId = request.TargetUserId,
            FromRole = RatingRole.Volunteer,
            ToRole = RatingRole.Organizer,
            Score = request.Score,
            Comment = request.Comment
        };

        _ratingRepository.Add(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Organizer rates a volunteer who participated in their event.
    /// </summary>
    public async Task<Result> SubmitOrganizerRatingAsync(Guid organizerUserId, CreateRatingRequest request, CancellationToken cancellationToken = default)
    {
        // Validate score
        if (request.Score < 1 || request.Score > 5)
            return Result.Failure(new Error("Rating.InvalidScore", "Score must be between 1 and 5."));

        // Validate comment length
        if (request.Comment != null && request.Comment.Length > 2000)
            return Result.Failure(new Error("Rating.CommentTooLong", "Comment must not exceed 2000 characters."));

        // Cannot rate self
        if (organizerUserId == request.TargetUserId)
            return Result.Failure(new Error("Rating.SelfRating", "You cannot rate yourself."));

        // Get event and check status
        var ev = await _eventRepository.GetDetailsByIdAsync(request.EventId, cancellationToken);
        if (ev == null)
            return Result.Failure(Error.NotFound);
        if (ev.Status != EventStatus.Completed)
            return Result.Failure(new Error("Rating.EventNotCompleted", "You can only rate after the event is completed."));

        // Organizer must own the event
        if (ev.OrganizerId != organizerUserId)
            return Result.Failure(Error.Unauthorized);

        // Resolve target volunteer profile from userId (approval/attendance repos use profileId)
        var targetProfile = await _profileRepository.GetByUserIdWithDetailsAsync(request.TargetUserId, cancellationToken);
        if (targetProfile == null)
            return Result.Failure(new Error("Rating.TargetNotFound", "Target volunteer profile not found."));

        // Validate target volunteer had approved application
        var isApproved = await _appRepository.IsApprovedAsync(request.EventId, targetProfile.Id, cancellationToken);
        if (!isApproved)
            return Result.Failure(new Error("Rating.VolunteerNotApproved", "The volunteer must have an approved application for this event."));

        // Validate target volunteer had approved attendance
        var hasAttendance = await _attendanceRepository.HasApprovedAttendanceAsync(request.EventId, targetProfile.Id, cancellationToken);
        if (!hasAttendance)
            return Result.Failure(new Error("Rating.VolunteerNoAttendance", "The volunteer must have approved attendance for this event."));

        // Prevent duplicate rating
        var exists = await _ratingRepository.ExistsAsync(request.EventId, organizerUserId, request.TargetUserId, cancellationToken);
        if (exists)
            return Result.Failure(new Error("Rating.Duplicate", "You have already rated this volunteer for this event."));

        var rating = new Rating
        {
            EventId = request.EventId,
            FromUserId = organizerUserId,
            ToUserId = request.TargetUserId,
            FromRole = RatingRole.Organizer,
            ToRole = RatingRole.Volunteer,
            Score = request.Score,
            Comment = request.Comment
        };

        _ratingRepository.Add(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<RatingResponse>>> GetMyRatingsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var ratings = await _ratingRepository.GetByUserIdAsync(userId, cancellationToken);
        return Result.Success(ratings.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<RatingResponse>>> GetRatingsByEventAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default)
    {
        // Validate organizer owns the event
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerUserId)
            return Result.Failure<List<RatingResponse>>(Error.Unauthorized);

        var ratings = await _ratingRepository.GetByEventIdAsync(eventId, cancellationToken);
        return Result.Success(ratings.Select(MapToResponse).ToList());
    }

    public async Task<Result<RatingSummaryResponse>> GetRatingSummaryAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var ratings = await _ratingRepository.GetRatingsForUserAsync(targetUserId, cancellationToken);

        var summary = new RatingSummaryResponse
        {
            TargetUserId = targetUserId,
            AverageScore = ratings.Count > 0 ? Math.Round(ratings.Average(r => r.Score), 2) : 0,
            TotalRatings = ratings.Count
        };

        return Result.Success(summary);
    }

    private static RatingResponse MapToResponse(Rating r)
    {
        return new RatingResponse
        {
            Id = r.Id,
            EventId = r.EventId,
            FromUserId = r.FromUserId,
            ToUserId = r.ToUserId,
            FromRole = r.FromRole.ToString(),
            ToRole = r.ToRole.ToString(),
            Score = r.Score,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
