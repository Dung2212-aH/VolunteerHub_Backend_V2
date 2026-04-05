namespace VolunteerHub.Contracts.Responses;

public class SponsorProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public List<ContactPersonResponse> ContactPersons { get; set; } = new();
}

public class ContactPersonResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Role { get; set; }
}

public class SponsorshipPackageResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Benefits { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class EventSponsorResponse
{
    public Guid Id { get; set; }
    public Guid SponsorProfileId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Guid EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public Guid? SponsorshipPackageId { get; set; }
    public string? PackageName { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DisplayPriority { get; set; }
    public bool IsPubliclyVisible { get; set; }
    public string? Note { get; set; }
    public string? RejectionReason { get; set; }
}

public class SponsorContributionResponse
{
    public Guid Id { get; set; }
    public Guid EventSponsorId { get; set; }
    public Guid SponsorProfileId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Description { get; set; }
    public DateTime ContributedAt { get; set; }
    public string? ReceiptReference { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Lightweight sponsor info for public event pages.
/// </summary>
public class PublicEventSponsorResponse
{
    public string CompanyName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? PackageName { get; set; }
    public int DisplayPriority { get; set; }
}
