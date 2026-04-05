using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Rating;

namespace VolunteerHub.Application.Abstractions;

public interface IRatingService
{
    Task<Result> SubmitVolunteerRatingAsync(Guid volunteerUserId, CreateRatingRequest request, CancellationToken cancellationToken = default);
    Task<Result> SubmitOrganizerRatingAsync(Guid organizerUserId, CreateRatingRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<RatingResponse>>> GetMyRatingsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<RatingResponse>>> GetRatingsByEventAsync(Guid organizerUserId, Guid eventId, CancellationToken cancellationToken = default);
    Task<Result<RatingSummaryResponse>> GetRatingSummaryAsync(Guid targetUserId, CancellationToken cancellationToken = default);
}
