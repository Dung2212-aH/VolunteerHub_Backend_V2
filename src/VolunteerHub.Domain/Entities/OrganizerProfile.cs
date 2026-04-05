using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class OrganizerProfile : AuditableEntity, ISoftDeletable
{
    public Guid UserId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mission { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public OrganizerVerificationStatus VerificationStatus { get; set; } = OrganizerVerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }
    public string RejectionReason { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<OrganizerLegalDocument> LegalDocuments { get; set; } = new List<OrganizerLegalDocument>();
    public ICollection<OrganizerVerificationReview> VerificationReviews { get; set; } = new List<OrganizerVerificationReview>();
}
