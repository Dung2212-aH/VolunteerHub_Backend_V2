using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class OrganizerLegalDocument : AuditableEntity
{
    public Guid OrganizerProfileId { get; set; }
    public OrganizerProfile OrganizerProfile { get; set; } = null!;
    
    public string DocumentType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    public DateTime UploadedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewerId { get; set; }
    public string Notes { get; set; } = string.Empty;
}
