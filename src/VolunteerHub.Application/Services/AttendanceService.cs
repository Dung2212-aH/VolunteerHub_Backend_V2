using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Application.Helpers;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IApplicationApprovalRepository _appRepository;
    private readonly IUnitOfWork _unitOfWork;

    private const double MaxGpsDistanceKm = 0.5; // 500 meters
    private readonly TimeSpan _checkInWindowBefore = TimeSpan.FromHours(1);
    private readonly TimeSpan _checkInWindowAfter = TimeSpan.FromHours(1);

    public AttendanceService(
        IAttendanceRepository attendanceRepository, 
        IEventRepository eventRepository, 
        IApplicationApprovalRepository appRepository,
        IUnitOfWork unitOfWork)
    {
        _attendanceRepository = attendanceRepository;
        _eventRepository = eventRepository;
        _appRepository = appRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CreateShiftAsync(Guid organizerId, Guid eventId, CreateShiftRequest request, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId)
            return Result.Failure(Error.NotFound);

        if (request.StartTime >= request.EndTime) return Result.Failure(new Error("Shift.InvalidDates", "Start must be before End."));

        var shift = new EventShift
        {
            EventId = eventId,
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxVolunteers = request.MaxVolunteers
        };

        _attendanceRepository.AddShift(shift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AssignVolunteerAsync(Guid organizerId, Guid shiftId, Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var shift = await _attendanceRepository.GetShiftByIdAsync(shiftId, cancellationToken);
        if (shift == null) return Result.Failure(Error.NotFound);

        var ev = await _eventRepository.GetDetailsByIdAsync(shift.EventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId) return Result.Failure(Error.NotFound);

        // Required rule step: Volunteer must hold an Approved application for the event
        var isApproved = await _appRepository.IsApprovedAsync(shift.EventId, volunteerProfileId, cancellationToken);
        if (!isApproved) return Result.Failure(new Error("Assignment.NotApproved", "Volunteer must have an Approved application for this event."));

        var exists = await _attendanceRepository.IsValidAssignmentAsync(shiftId, volunteerProfileId, cancellationToken);
        if (exists) return Result.Failure(new Error("Assignment.Exists", "Volunteer already assigned."));

        var assignment = new ShiftAssignment
        {
            EventShiftId = shiftId,
            VolunteerProfileId = volunteerProfileId,
            Status = "Assigned"
        };

        _attendanceRepository.AddAssignment(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CheckInAsync(Guid volunteerProfileId, CheckInRequest request, CancellationToken cancellationToken = default)
    {
        var isAssigned = await _attendanceRepository.IsValidAssignmentAsync(request.EventShiftId, volunteerProfileId, cancellationToken);
        if (!isAssigned) return Result.Failure(new Error("Attendance.NotAssigned", "You must be assigned to this shift to check in."));

        var shift = await _attendanceRepository.GetShiftByIdAsync(request.EventShiftId, cancellationToken);
        if (shift == null) return Result.Failure(Error.NotFound);

        var ev = await _eventRepository.GetDetailsByIdAsync(shift.EventId, cancellationToken);
        
        var now = DateTime.UtcNow;
        if (now < shift.StartTime.Subtract(_checkInWindowBefore) || now > shift.StartTime.Add(_checkInWindowAfter))
        {
            return Result.Failure(new Error("Attendance.InvalidTimeWindow", "Check-in is not currently open for this shift."));
        }

        if (!Enum.TryParse<CheckInMethod>(request.Method, true, out var methodType))
            methodType = CheckInMethod.None;

        if (methodType == CheckInMethod.GPS && request.Latitude.HasValue && request.Longitude.HasValue && ev != null && ev.Latitude.HasValue && ev.Longitude.HasValue)
        {
            var dist = LocationHelper.CalculateDistanceKm(ev.Latitude.Value, ev.Longitude.Value, request.Latitude.Value, request.Longitude.Value);
            if (dist > MaxGpsDistanceKm) return Result.Failure(new Error("Attendance.OutOfRange", "You are too far from the event location."));
        }

        var existingRecord = await _attendanceRepository.GetRecordAsync(request.EventShiftId, volunteerProfileId, cancellationToken);
        if (existingRecord != null && existingRecord.Status != AttendanceStatus.Pending)
        {
            return Result.Failure(new Error("Attendance.Duplicate", "You have already checked in."));
        }

        if (existingRecord == null)
        {
            var record = new AttendanceRecord
            {
                EventId = shift.EventId,
                EventShiftId = shift.Id,
                VolunteerProfileId = volunteerProfileId,
                CheckInAt = now,
                CheckInMethod = methodType,
                CheckInLatitude = request.Latitude,
                CheckInLongitude = request.Longitude,
                Status = AttendanceStatus.CheckedIn
            };
            _attendanceRepository.AddAttendanceRecord(record);
        }
        else
        {
            existingRecord.CheckInAt = now;
            existingRecord.CheckInMethod = methodType;
            existingRecord.CheckInLatitude = request.Latitude;
            existingRecord.CheckInLongitude = request.Longitude;
            existingRecord.Status = AttendanceStatus.CheckedIn;
            _attendanceRepository.UpdateAttendanceRecord(existingRecord);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CheckOutAsync(Guid volunteerProfileId, CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        var shift = await _attendanceRepository.GetShiftByIdAsync(request.EventShiftId, cancellationToken);
        if (shift == null) return Result.Failure(Error.NotFound);

        var record = await _attendanceRepository.GetRecordAsync(request.EventShiftId, volunteerProfileId, cancellationToken);
        if (record == null || record.Status != AttendanceStatus.CheckedIn)
        {
            return Result.Failure(new Error("Attendance.NoCheckIn", "You must be checked in to check out."));
        }

        if (!Enum.TryParse<CheckInMethod>(request.Method, true, out var methodType))
            methodType = CheckInMethod.None;

        var now = DateTime.UtcNow;
        record.CheckOutAt = now;
        record.CheckOutMethod = methodType;
        record.CheckOutLatitude = request.Latitude;
        record.CheckOutLongitude = request.Longitude;

        if (record.CheckInAt.HasValue && record.CheckOutAt.HasValue)
        {
            var duration = record.CheckOutAt.Value - record.CheckInAt.Value;
            var maxShiftDuration = shift.EndTime - shift.StartTime;

            if (duration.TotalMinutes > 15) 
            {
                var approvedHours = Math.Min(duration.TotalHours, maxShiftDuration.TotalHours);
                record.ApprovedHours = Math.Round(approvedHours, 2);
                
                if (now > shift.EndTime.AddHours(2))
                    record.Status = AttendanceStatus.NeedsReview;
                else
                    record.Status = AttendanceStatus.CheckedOut;
            }
            else
            {
                record.Status = AttendanceStatus.NeedsReview;
            }
        }

        _attendanceRepository.UpdateAttendanceRecord(record);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ManualOverrideAsync(Guid organizerId, Guid shiftId, ManualOverrideRequest request, CancellationToken cancellationToken = default)
    {
        var shift = await _attendanceRepository.GetShiftByIdAsync(shiftId, cancellationToken);
        if (shift == null) return Result.Failure(Error.NotFound);

        var ev = await _eventRepository.GetDetailsByIdAsync(shift.EventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId) return Result.Failure(Error.NotFound);

        var record = await _attendanceRepository.GetRecordAsync(shiftId, request.VolunteerProfileId, cancellationToken);
        if (record == null)
        {
            record = new AttendanceRecord
            {
                EventId = shift.EventId,
                EventShiftId = shiftId,
                VolunteerProfileId = request.VolunteerProfileId,
                CheckInMethod = CheckInMethod.Manual,
                CheckOutMethod = CheckInMethod.Manual
            };
            _attendanceRepository.AddAttendanceRecord(record);
        }

        if (!Enum.TryParse<AttendanceStatus>(request.NewStatus, true, out var statusType))
            return Result.Failure(new Error("Attendance.InvalidStatus", "Invalid attendance status payload."));

        record.Status = statusType;
        record.CheckInAt = request.CheckInAt;
        record.CheckOutAt = request.CheckOutAt;
        record.OverrideReason = request.Reason;
        record.OverrideByUserId = organizerId;
        record.OverrideAt = DateTime.UtcNow;

        if (record.CheckInAt.HasValue && record.CheckOutAt.HasValue)
            record.ApprovedHours = Math.Round((record.CheckOutAt.Value - record.CheckInAt.Value).TotalHours, 2);

        _attendanceRepository.UpdateAttendanceRecord(record);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result<List<EventShiftResponse>>> GetEventShiftsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var shifts = await _attendanceRepository.GetShiftsByEventAsync(eventId, cancellationToken);
        return Result.Success(shifts.Select(s => new EventShiftResponse
        {
            Id = s.Id,
            EventId = s.EventId,
            Title = s.Title,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            MaxVolunteers = s.MaxVolunteers
        }).ToList());
    }

    public async Task<Result<List<AttendanceRecordResponse>>> GetMyAttendanceAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var records = await _attendanceRepository.GetRecordsByVolunteerAsync(volunteerProfileId, cancellationToken);
        return Result.Success(records.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<AttendanceRecordResponse>>> GetEventAttendanceAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId) return Result.Failure<List<AttendanceRecordResponse>>(Error.NotFound);

        var records = await _attendanceRepository.GetRecordsByEventAsync(eventId, cancellationToken);
        return Result.Success(records.Select(MapToResponse).ToList());
    }

    private AttendanceRecordResponse MapToResponse(AttendanceRecord record)
    {
        return new AttendanceRecordResponse
        {
            Id = record.Id,
            EventId = record.EventId,
            EventShiftId = record.EventShiftId,
            VolunteerProfileId = record.VolunteerProfileId,
            EventTitle = record.Event?.Title ?? string.Empty,
            ShiftTitle = record.EventShift?.Title ?? string.Empty,
            CheckInAt = record.CheckInAt,
            CheckOutAt = record.CheckOutAt,
            Status = record.Status.ToString(),
            ApprovedHours = record.ApprovedHours
        };
    }
}
