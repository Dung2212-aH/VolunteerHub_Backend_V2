using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IAttendanceRepository
{
    void AddShift(EventShift shift);
    Task<EventShift?> GetShiftByIdAsync(Guid shiftId, CancellationToken cancellationToken = default);
    Task<List<EventShift>> GetShiftsByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    
    void AddAssignment(ShiftAssignment assignment);
    Task<bool> IsValidAssignmentAsync(Guid shiftId, Guid profileId, CancellationToken cancellationToken = default);
    
    void AddAttendanceRecord(AttendanceRecord record);
    void UpdateAttendanceRecord(AttendanceRecord record);
    Task<AttendanceRecord?> GetRecordAsync(Guid shiftId, Guid profileId, CancellationToken cancellationToken = default);
    Task<List<AttendanceRecord>> GetRecordsByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<List<AttendanceRecord>> GetRecordsByVolunteerAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<bool> HasApprovedAttendanceAsync(Guid eventId, Guid profileId, CancellationToken cancellationToken = default);
    Task<double> GetTotalApprovedHoursAsync(Guid profileId, CancellationToken cancellationToken = default);
}
