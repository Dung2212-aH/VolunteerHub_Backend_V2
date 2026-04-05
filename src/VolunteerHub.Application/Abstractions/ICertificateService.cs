using VolunteerHub.Contracts.Recognition;
using VolunteerHub.Application.Common;

namespace VolunteerHub.Application.Abstractions;

public interface ICertificateService
{
    Task<Result<CertificateResponse>> IssueCertificateAsync(IssueCertificateRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<CertificateResponse>>> GetMyCertificatesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
    Task<Result<CertificateResponse>> GetCertificateByIdAsync(Guid profileId, Guid id, CancellationToken cancellationToken = default);
    Task<Result> RevokeCertificateAsync(Guid id, RevokeCertificateRequest request, CancellationToken cancellationToken = default);
    Task<Result<CertificateVerificationResponse>> VerifyCertificateAsync(string code, CancellationToken cancellationToken = default);
}
