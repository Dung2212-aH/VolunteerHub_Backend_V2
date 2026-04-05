namespace VolunteerHub.Domain.Entities;

public enum AttendanceStatus
{
    Pending,
    CheckedIn,
    CheckedOut,
    Late,
    Absent,
    NeedsReview,
    Approved,
    Rejected
}
