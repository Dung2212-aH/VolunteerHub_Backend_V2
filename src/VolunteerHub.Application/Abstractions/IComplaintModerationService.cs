using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IComplaintModerationService
{
    Task<Result<List<AdminFeedbackReportResponse>>> GetPendingReportsAsync(CancellationToken cancellationToken = default);
    Task<Result> ResolveReportAsync(Guid adminUserId, Guid reportId, ResolveFeedbackReportRequest request, CancellationToken cancellationToken = default);
}