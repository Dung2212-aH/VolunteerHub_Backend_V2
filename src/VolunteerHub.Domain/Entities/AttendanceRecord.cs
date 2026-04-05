using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class AttendanceRecord : AuditableEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid EventShiftId { get; set; }
    public EventShift EventShift { get; set; } = null!;

    public Guid VolunteerProfileId { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;

    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }

    public CheckInMethod CheckInMethod { get; set; } = CheckInMethod.None;
    public CheckInMethod CheckOutMethod { get; set; } = CheckInMethod.None;

    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Pending;

    // Computed approved hours
    public double? ApprovedHours { get; set; }

    // Audit trail for manual overrides
    public string OverrideReason { get; set; } = string.Empty;
    public Guid? OverrideByUserId { get; set; } 
    public DateTime? OverrideAt { get; set; }
}
