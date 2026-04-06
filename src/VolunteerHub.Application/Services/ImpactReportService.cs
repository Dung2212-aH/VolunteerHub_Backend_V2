using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Services;

public class ImpactReportService : IImpactReportService
{
    private readonly IAdminRepository _adminRepository;

    public ImpactReportService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<Result<AdminDashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var response = new AdminDashboardResponse
        {
            TotalVolunteers = await _adminRepository.CountVolunteersAsync(cancellationToken),
            TotalOrganizers = await _adminRepository.CountOrganizersAsync(cancellationToken),
            TotalPublishedEvents = await _adminRepository.CountPublishedEventsAsync(cancellationToken),
            TotalCompletedEvents = await _adminRepository.CountCompletedEventsAsync(cancellationToken),
            TotalCertificatesIssued = await _adminRepository.CountCertificatesIssuedAsync(cancellationToken),
            TotalBadgesAwarded = await _adminRepository.CountBadgesAwardedAsync(cancellationToken),
            TotalSponsorsApproved = await _adminRepository.CountApprovedSponsorsAsync(cancellationToken),
            TotalDonationsConfirmed = await _adminRepository.CountConfirmedDonationsAsync(cancellationToken),
            TotalPendingOrganizerVerifications = await _adminRepository.CountPendingOrganizerVerificationsAsync(cancellationToken),
            TotalPendingSponsorProfiles = await _adminRepository.CountPendingSponsorProfilesAsync(cancellationToken),
            TotalPendingFeedbackReports = await _adminRepository.CountPendingFeedbackReportsAsync(cancellationToken)
        };

        return Result.Success(response);
    }
}