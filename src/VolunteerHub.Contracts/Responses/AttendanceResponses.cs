namespace VolunteerHub.Contracts.Responses;

public class AttendanceRecordResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid EventShiftId { get; set; }
    public Guid VolunteerProfileId { get; set; }
    
    public string EventTitle { get; set; } = string.Empty;
    public string ShiftTitle { get; set; } = string.Empty;
    
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public double? ApprovedHours { get; set; }
}

public class EventShiftResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxVolunteers { get; set; }
}
