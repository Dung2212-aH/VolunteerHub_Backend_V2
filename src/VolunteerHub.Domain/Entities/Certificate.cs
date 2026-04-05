using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class Certificate : AuditableEntity, ISoftDeletable
{
    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;

    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string CertificateNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    
    public string VerificationCode { get; set; } = string.Empty;
    public string QrCodeContent { get; set; } = string.Empty;
    
    public string? PdfPath { get; set; } // Nullable/placeholder until file generation exists
    
    public CertificateStatus Status { get; set; } = CertificateStatus.Active;
    
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
