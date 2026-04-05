using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class Event : AuditableEntity, ISoftDeletable
{
    public Guid OrganizerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Capacity { get; set; }
    public EventStatus Status { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<EventSkillRequirement> SkillRequirements { get; set; } = new List<EventSkillRequirement>();
    public ICollection<EventShift> Shifts { get; set; } = new List<EventShift>();
    public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
}
