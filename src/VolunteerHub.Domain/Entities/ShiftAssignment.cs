using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class ShiftAssignment : AuditableEntity, ISoftDeletable
{
    public Guid EventShiftId { get; set; }
    public EventShift EventShift { get; set; } = null!;

    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;

    public string Status { get; set; } = "Assigned";
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
