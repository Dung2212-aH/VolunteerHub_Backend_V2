using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class CertificateEligibilityService : ICertificateEligibilityService
{
    private readonly IEventRepository _eventRepository;
    private readonly IApplicationApprovalRepository _appRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IRecognitionRepository _recognitionRepository;

    public CertificateEligibilityService(
        IEventRepository eventRepository,
        IApplicationApprovalRepository appRepository,
        IAttendanceRepository attendanceRepository,
        IRecognitionRepository recognitionRepository)
    {
        _eventRepository = eventRepository;
        _appRepository = appRepository;
        _attendanceRepository = attendanceRepository;
        _recognitionRepository = recognitionRepository;
    }

    public async Task<Result> CheckEligibilityAsync(Guid volunteerProfileId, Guid eventId, CancellationToken cancellationToken = default)
    {
        // 1. the related event exists and 2. event status is Completed
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null)
            return Result.Failure(new Error("Certificate.EventNotFound", "Event not found."));
        
        if (ev.Status != EventStatus.Completed)
            return Result.Failure(new Error("Certificate.EventNotCompleted", "Event must be completed to issue a certificate."));

        // 3. volunteer has an approved application for that event
        var isApprovedApp = await _appRepository.IsApprovedAsync(eventId, volunteerProfileId, cancellationToken);
        if (!isApprovedApp)
            return Result.Failure(new Error("Certificate.NoApprovedApplication", "Volunteer must have an approved application."));

        // 4. volunteer has an approved attendance record for that event
        var hasApprovedAttendance = await _attendanceRepository.HasApprovedAttendanceAsync(eventId, volunteerProfileId, cancellationToken);
        if (!hasApprovedAttendance)
            return Result.Failure(new Error("Certificate.NoApprovedAttendance", "Volunteer must have an approved attendance record."));

        // 5. no existing active certificate for the same volunteer + event pair
        var activeCert = await _recognitionRepository.GetActiveCertificateAsync(volunteerProfileId, eventId, cancellationToken);
        if (activeCert != null)
            return Result.Failure(new Error("Certificate.AlreadyIssued", "An active certificate has already been issued for this event."));

        return Result.Success();
    }
}
