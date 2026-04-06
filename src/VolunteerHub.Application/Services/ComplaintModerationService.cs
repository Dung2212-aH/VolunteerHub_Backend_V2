using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Application.Services;

public class ComplaintModerationService : IComplaintModerationService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IAdminAuditService _adminAuditService;
    private readonly IUnitOfWork _unitOfWork;

    public ComplaintModerationService(
        IAdminRepository adminRepository,
        IAdminAuditService adminAuditService,
        IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _adminAuditService = adminAuditService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<AdminFeedbackReportResponse>>> GetPendingReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _adminRepository.GetPendingFeedbackReportsAsync(cancellationToken);
        var response = reports.Select(r => new AdminFeedbackReportResponse
        {
            Id = r.Id,
            EventId = r.EventId,
            ReporterUserId = r.ReporterUserId,
            TargetUserId = r.TargetUserId,
            Reason = r.Reason,
            Description = r.Description,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            ResolvedAt = r.ResolvedAt
        }).ToList();

        return Result.Success(response);
    }

    public async Task<Result> ResolveReportAsync(Guid adminUserId, Guid reportId, ResolveFeedbackReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _adminRepository.GetFeedbackReportByIdAsync(reportId, cancellationToken);
        if (report == null)
            return Result.Failure(Error.NotFound);

        if (report.Status == ReportStatus.Resolved || report.Status == ReportStatus.Rejected)
            return Result.Failure(new Error("Admin.ReportAlreadyClosed", "This report has already been closed."));

        report.Status = request.Approve ? ReportStatus.Resolved : ReportStatus.Rejected;
        report.ResolvedAt = DateTime.UtcNow;

        _adminRepository.UpdateFeedbackReport(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _adminAuditService.LogAsync(
            adminUserId,
            request.Approve ? "ResolveComplaint" : "RejectComplaint",
            nameof(VolunteerHub.Domain.Entities.FeedbackReport),
            report.Id,
            request.ResolutionNote,
            cancellationToken);

        return Result.Success();
    }
}