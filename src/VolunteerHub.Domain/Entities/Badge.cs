using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class Badge : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
