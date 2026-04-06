using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IImpactReportService
{
    Task<Result<AdminDashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default);
}