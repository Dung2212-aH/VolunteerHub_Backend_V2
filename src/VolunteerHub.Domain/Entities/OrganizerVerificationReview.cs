using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class OrganizerVerificationReview : AuditableEntity
{
    public Guid OrganizerProfileId { get; set; }
    public OrganizerProfile OrganizerProfile { get; set; } = null!;
    
    public OrganizerVerificationStatus PreviousStatus { get; set; }
    public OrganizerVerificationStatus NewStatus { get; set; }
    public Guid ReviewerId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}
