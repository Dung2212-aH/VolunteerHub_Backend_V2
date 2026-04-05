using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class EventSkillRequirement : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public string SkillName { get; set; } = string.Empty;
}
