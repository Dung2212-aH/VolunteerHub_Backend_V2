using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class SponsorshipPackage : AuditableEntity, ISoftDeletable
{
    /// <summary>The event this package belongs to.</summary>
    public Guid EventId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Monetary amount required for this package (must be > 0).</summary>
    public decimal Amount { get; set; }

    /// <summary>Comma-separated or JSON list of benefits included.</summary>
    public string Benefits { get; set; } = string.Empty;

    /// <summary>Lower value = higher visual priority on event page.</summary>
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Event Event { get; set; } = null!;
    public ICollection<EventSponsor> EventSponsors { get; set; } = new List<EventSponsor>();
}
