using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class VolunteerSkill : BaseEntity
{
    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile Profile { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}
