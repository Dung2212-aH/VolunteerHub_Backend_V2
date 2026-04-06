using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SkillCatalogItem>> GetSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SkillCatalogItems
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SkillCatalogItem>> GetActiveSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SkillCatalogItems
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<SkillCatalogItem?> GetSkillByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SkillCatalogItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SkillCatalogItem?> GetSkillByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.SkillCatalogItems.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public void AddSkill(SkillCatalogItem skill)
    {
        _context.SkillCatalogItems.Add(skill);
    }

    public void UpdateSkill(SkillCatalogItem skill)
    {
        _context.SkillCatalogItems.Update(skill);
    }

    public void AddAdminActionLog(AdminActionLog log)
    {
        _context.AdminActionLogs.Add(log);
    }

    public async Task<List<FeedbackReport>> GetPendingFeedbackReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FeedbackReports
            .Where(x => x.Status == ReportStatus.Pending || x.Status == ReportStatus.UnderReview)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FeedbackReport?> GetFeedbackReportByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FeedbackReports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void UpdateFeedbackReport(FeedbackReport report)
    {
        _context.FeedbackReports.Update(report);
    }

    public async Task<int> CountVolunteersAsync(CancellationToken cancellationToken = default)
        => await _context.VolunteerProfiles.CountAsync(cancellationToken);

    public async Task<int> CountOrganizersAsync(CancellationToken cancellationToken = default)
        => await _context.OrganizerProfiles.CountAsync(cancellationToken);

    public async Task<int> CountPublishedEventsAsync(CancellationToken cancellationToken = default)
        => await _context.Events.CountAsync(x => x.Status.ToString() == "Published", cancellationToken);

    public async Task<int> CountCompletedEventsAsync(CancellationToken cancellationToken = default)
        => await _context.Events.CountAsync(x => x.Status.ToString() == "Completed", cancellationToken);

    public async Task<int> CountCertificatesIssuedAsync(CancellationToken cancellationToken = default)
        => await _context.Certificates.CountAsync(cancellationToken);

    public async Task<int> CountBadgesAwardedAsync(CancellationToken cancellationToken = default)
        => await _context.VolunteerBadges.CountAsync(cancellationToken);

    public async Task<int> CountApprovedSponsorsAsync(CancellationToken cancellationToken = default)
        => await _context.SponsorProfiles.CountAsync(x => x.Status == SponsorProfileStatus.Approved, cancellationToken);

    public async Task<int> CountConfirmedDonationsAsync(CancellationToken cancellationToken = default)
        => await _context.SponsorContributions.CountAsync(cancellationToken);

    public async Task<int> CountPendingOrganizerVerificationsAsync(CancellationToken cancellationToken = default)
        => await _context.OrganizerProfiles.CountAsync(x => x.VerificationStatus == OrganizerVerificationStatus.Pending, cancellationToken);

    public async Task<int> CountPendingSponsorProfilesAsync(CancellationToken cancellationToken = default)
        => await _context.SponsorProfiles.CountAsync(x => x.Status == SponsorProfileStatus.PendingApproval, cancellationToken);

    public async Task<int> CountPendingFeedbackReportsAsync(CancellationToken cancellationToken = default)
        => await _context.FeedbackReports.CountAsync(x => x.Status == ReportStatus.Pending || x.Status == ReportStatus.UnderReview, cancellationToken);
}