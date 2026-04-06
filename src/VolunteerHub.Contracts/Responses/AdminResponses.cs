namespace VolunteerHub.Contracts.Responses;

public class SkillCatalogItemResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class AdminFeedbackReportResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class AdminDashboardResponse
{
    public int TotalVolunteers { get; set; }
    public int TotalOrganizers { get; set; }
    public int TotalPublishedEvents { get; set; }
    public int TotalCompletedEvents { get; set; }
    public int TotalCertificatesIssued { get; set; }
    public int TotalBadgesAwarded { get; set; }
    public int TotalSponsorsApproved { get; set; }
    public int TotalDonationsConfirmed { get; set; }
    public int TotalPendingOrganizerVerifications { get; set; }
    public int TotalPendingSponsorProfiles { get; set; }
    public int TotalPendingFeedbackReports { get; set; }
}