using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class EventSponsor : AuditableEntity
{
    public Guid SponsorProfileId { get; set; }
    public Guid EventId { get; set; }
    public Guid? SponsorshipPackageId { get; set; }

    public EventSponsorStatus Status { get; set; } = EventSponsorStatus.Pending;

    /// <summary>Lower value = higher visual priority on the event page.</summary>
    public int DisplayPriority { get; set; }

    /// <summary>Whether the sponsor is visible on the public event page.</summary>
    public bool IsPubliclyVisible { get; set; }

    public string? Note { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation
    public SponsorProfile SponsorProfile { get; set; } = null!;
    public Event Event { get; set; } = null!;
    public SponsorshipPackage? SponsorshipPackage { get; set; }
    public ICollection<SponsorContribution> Contributions { get; set; } = new List<SponsorContribution>();
}
