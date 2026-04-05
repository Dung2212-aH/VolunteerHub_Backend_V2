using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

// ── Sponsor Profile ──────────────────────────────────────────────────

public class CreateSponsorProfileRequest
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    public List<CreateSponsorContactPersonRequest> ContactPersons { get; set; } = new();
}

public class UpdateSponsorProfileRequest
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    public List<CreateSponsorContactPersonRequest> ContactPersons { get; set; } = new();
}

public class CreateSponsorContactPersonRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Role { get; set; }
}

// ── Sponsorship Package ──────────────────────────────────────────────

public class CreateSponsorshipPackageRequest : IValidatableObject
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [MaxLength(4000)]
    public string Benefits { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Amount <= 0)
            yield return new ValidationResult("Amount must be > 0.", new[] { nameof(Amount) });
    }
}

public class UpdateSponsorshipPackageRequest : IValidatableObject
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [MaxLength(4000)]
    public string Benefits { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Amount <= 0)
            yield return new ValidationResult("Amount must be > 0.", new[] { nameof(Amount) });
    }
}

// ── Event Sponsor (Request to sponsor an event) ──────────────────────

public class SponsorEventRequest
{
    [Required]
    public Guid EventId { get; set; }

    public Guid? SponsorshipPackageId { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }
}

// ── Approve / Reject ─────────────────────────────────────────────────

public class ApproveRejectEventSponsorRequest
{
    [Required]
    public bool Approve { get; set; }

    [MaxLength(1000)]
    public string? Reason { get; set; }

    public bool IsPubliclyVisible { get; set; } = true;
    public int DisplayPriority { get; set; }
}

public class ApproveSponsorProfileRequest
{
    [Required]
    public bool Approve { get; set; }

    [MaxLength(1000)]
    public string? Reason { get; set; }
}

// ── Contribution ─────────────────────────────────────────────────────

public class RecordContributionRequest : IValidatableObject
{
    [Required]
    public Guid EventSponsorId { get; set; }

    [Required]
    public string Type { get; set; } = "Monetary"; // Monetary | InKind

    [Range(0, double.MaxValue, ErrorMessage = "Value must be >= 0.")]
    public decimal Value { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime? ContributedAt { get; set; }

    [MaxLength(200)]
    public string? ReceiptReference { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Value < 0)
            yield return new ValidationResult("Value must be non-negative.", new[] { nameof(Value) });
    }
}
