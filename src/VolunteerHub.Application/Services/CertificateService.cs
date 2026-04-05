using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Recognition;
using VolunteerHub.Application.Common;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class CertificateService : ICertificateService
{
    private readonly IRecognitionRepository _repository;
    private readonly ICertificateEligibilityService _eligibilityService;
    private readonly IBadgeService _badgeService;
    private readonly IEventRepository _eventRepository;
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public CertificateService(
        IRecognitionRepository repository,
        ICertificateEligibilityService eligibilityService,
        IBadgeService badgeService,
        IEventRepository eventRepository,
        IVolunteerProfileRepository profileRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _eligibilityService = eligibilityService;
        _badgeService = badgeService;
        _eventRepository = eventRepository;
        _profileRepository = profileRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CertificateResponse>> IssueCertificateAsync(IssueCertificateRequest request, CancellationToken cancellationToken = default)
    {
        // Issue operation must be idempotent. If an active certificate already exists, just return it.
        var existingActive = await _repository.GetActiveCertificateAsync(request.VolunteerProfileId, request.EventId, cancellationToken);
        if (existingActive != null)
        {
            return Result.Success(MapToResponse(existingActive));
        }

        var eligibilityResult = await _eligibilityService.CheckEligibilityAsync(request.VolunteerProfileId, request.EventId, cancellationToken);
        if (!eligibilityResult.IsSuccess)
        {
            return Result.Failure<CertificateResponse>(eligibilityResult.Error);
        }

        var ev = await _eventRepository.GetDetailsByIdAsync(request.EventId, cancellationToken);
        
        var verificationCode = Guid.NewGuid().ToString("N");

        var certificate = new Certificate
        {
            VolunteerProfileId = request.VolunteerProfileId,
            EventId = request.EventId,
            CertificateNumber = $"CERT-{DateTime.UtcNow:yyyyMM}-{verificationCode[..6].ToUpper()}",
            Title = $"Certificate of Participation: {ev!.Title}",
            IssuedAt = DateTime.UtcNow,
            VerificationCode = verificationCode,
            QrCodeContent = $"/certificate/verify/{verificationCode}",
            PdfPath = null, // Document generation deferred
            Status = CertificateStatus.Active
        };

        _repository.AddCertificate(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // evaluate badges
        await _badgeService.EvaluateBadgesAsync(request.VolunteerProfileId, cancellationToken);

        // Trigger notification — resolve profileId to Identity userId
        var volunteerProfile = await _profileRepository.GetByIdWithDetailsAsync(request.VolunteerProfileId, cancellationToken);
        if (volunteerProfile != null)
        {
            await _notificationService.NotifyCertificateIssuedAsync(
                volunteerProfile.UserId, ev!.Title, certificate.Id, cancellationToken);
        }
        
        // Return fully loaded response
        certificate.Event = ev;
        return Result.Success(MapToResponse(certificate));
    }

    public async Task<Result<List<CertificateResponse>>> GetMyCertificatesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var list = await _repository.GetMyCertificatesAsync(volunteerProfileId, cancellationToken);
        return Result.Success(list.Select(MapToResponse).ToList());
    }

    public async Task<Result<CertificateResponse>> GetCertificateByIdAsync(Guid profileId, Guid id, CancellationToken cancellationToken = default)
    {
        var cert = await _repository.GetCertificateByIdAsync(id, cancellationToken);
        if (cert == null || cert.VolunteerProfileId != profileId)
            return Result.Failure<CertificateResponse>(Error.NotFound);

        return Result.Success(MapToResponse(cert));
    }

    public async Task<Result> RevokeCertificateAsync(Guid id, RevokeCertificateRequest request, CancellationToken cancellationToken = default)
    {
        var cert = await _repository.GetCertificateByIdAsync(id, cancellationToken);
        if (cert == null) return Result.Failure(Error.NotFound);

        if (string.IsNullOrWhiteSpace(request.RevocationReason))
            return Result.Failure(new Error("Certificate.ReasonRequired", "A revocation reason is required."));

        if (cert.Status == CertificateStatus.Revoked)
            return Result.Success(); // Idempotent

        cert.Status = CertificateStatus.Revoked;
        cert.RevokedAt = DateTime.UtcNow;
        cert.RevocationReason = request.RevocationReason;

        _repository.UpdateCertificate(cert);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CertificateVerificationResponse>> VerifyCertificateAsync(string code, CancellationToken cancellationToken = default)
    {
        var cert = await _repository.GetCertificateByVerificationCodeAsync(code, cancellationToken);
        
        if (cert == null)
            return Result.Failure<CertificateVerificationResponse>(new Error("Verification.NotFound", "Invalid verification code."));

        return Result.Success(new CertificateVerificationResponse
        {
            IsValid = cert.Status == CertificateStatus.Active, // Revoked fails validity check explicitly
            CertificateNumber = cert.CertificateNumber,
            VolunteerDisplayName = cert.VolunteerProfile?.FullName ?? "Unknown",
            EventTitle = cert.Event?.Title ?? "Unknown Event",
            IssuedAt = cert.IssuedAt,
            Status = cert.Status.ToString()
        });
    }

    private static CertificateResponse MapToResponse(Certificate cert)
    {
        return new CertificateResponse
        {
            Id = cert.Id,
            VolunteerProfileId = cert.VolunteerProfileId,
            EventId = cert.EventId,
            CertificateNumber = cert.CertificateNumber,
            Title = cert.Title,
            IssuedAt = cert.IssuedAt,
            VerificationCode = cert.VerificationCode,
            QrCodeContent = cert.QrCodeContent,
            PdfPath = cert.PdfPath,
            Status = cert.Status.ToString()
        };
    }
}
