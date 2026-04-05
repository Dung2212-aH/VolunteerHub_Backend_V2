using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Rating;

namespace VolunteerHub.Application.Abstractions;

public interface IFeedbackService
{
    Task<Result> SubmitReportAsync(Guid reporterUserId, CreateFeedbackReportRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<FeedbackReportResponse>>> GetMyReportsAsync(Guid reporterUserId, CancellationToken cancellationToken = default);
}
