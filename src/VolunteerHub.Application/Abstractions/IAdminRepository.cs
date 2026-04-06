using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IAdminRepository
{
    Task<List<SkillCatalogItem>> GetSkillsAsync(CancellationToken cancellationToken = default);
    Task<List<SkillCatalogItem>> GetActiveSkillsAsync(CancellationToken cancellationToken = default);
    Task<SkillCatalogItem?> GetSkillByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SkillCatalogItem?> GetSkillByCodeAsync(string code, CancellationToken cancellationToken = default);

    void AddSkill(SkillCatalogItem skill);
    void UpdateSkill(SkillCatalogItem skill);

    void AddAdminActionLog(AdminActionLog log);

    Task<List<FeedbackReport>> GetPendingFeedbackReportsAsync(CancellationToken cancellationToken = default);
    Task<FeedbackReport?> GetFeedbackReportByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void UpdateFeedbackReport(FeedbackReport report);

    Task<int> CountVolunteersAsync(CancellationToken cancellationToken = default);
    Task<int> CountOrganizersAsync(CancellationToken cancellationToken = default);
    Task<int> CountPublishedEventsAsync(CancellationToken cancellationToken = default);
    Task<int> CountCompletedEventsAsync(CancellationToken cancellationToken = default);
    Task<int> CountCertificatesIssuedAsync(CancellationToken cancellationToken = default);
    Task<int> CountBadgesAwardedAsync(CancellationToken cancellationToken = default);
    Task<int> CountApprovedSponsorsAsync(CancellationToken cancellationToken = default);
    Task<int> CountConfirmedDonationsAsync(CancellationToken cancellationToken = default);
    Task<int> CountPendingOrganizerVerificationsAsync(CancellationToken cancellationToken = default);
    Task<int> CountPendingSponsorProfilesAsync(CancellationToken cancellationToken = default);
    Task<int> CountPendingFeedbackReportsAsync(CancellationToken cancellationToken = default);
}