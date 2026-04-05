using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class VolunteerLanguage : BaseEntity
{
    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile Profile { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}
