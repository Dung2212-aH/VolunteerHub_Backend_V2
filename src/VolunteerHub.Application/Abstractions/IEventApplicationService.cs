using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IEventApplicationService
{
    Task<Result> ApplyAsync(Guid volunteerProfileId, ApplyToEventRequest request, CancellationToken cancellationToken = default);
    Task<Result> WithdrawAsync(Guid volunteerProfileId, Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result<ApplicationResponse>> GetApplicationAsync(Guid volunteerProfileId, Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result<List<ApplicationResponse>>> GetMyApplicationsAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
}

public interface IApplicationReviewService
{
    Task<Result> ApproveAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result> RejectAsync(Guid organizerId, Guid applicationId, ReviewApplicationRequest request, CancellationToken cancellationToken = default);
    Task<Result> WaitlistAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default);
    Task<Result> AddNoteAsync(Guid organizerId, Guid applicationId, AddReviewNoteRequest request, CancellationToken cancellationToken = default);
    
    Task<Result<List<ApplicantSummaryResponse>>> GetEventApplicationsAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default);
    Task<Result<ApplicantSummaryResponse>> GetApplicationDetailsAsync(Guid organizerId, Guid applicationId, CancellationToken cancellationToken = default);
}
