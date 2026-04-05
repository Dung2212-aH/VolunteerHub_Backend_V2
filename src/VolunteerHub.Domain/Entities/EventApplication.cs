using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class EventApplication : AuditableEntity, ISoftDeletable
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public string MotivationText { get; set; } = string.Empty;
    public string AvailabilityNote { get; set; } = string.Empty;

    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;

    public DateTime? WithdrawnAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<ApplicationReviewNote> ReviewNotes { get; set; } = new List<ApplicationReviewNote>();
    public ICollection<ApplicationDecisionHistory> DecisionHistory { get; set; } = new List<ApplicationDecisionHistory>();
}
