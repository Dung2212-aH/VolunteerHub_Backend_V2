using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IRecognitionRepository
{
    // Certificates
    void AddCertificate(Certificate certificate);
    void UpdateCertificate(Certificate certificate);
    Task<Certificate?> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Certificate?> GetCertificateByVerificationCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Certificate?> GetActiveCertificateAsync(Guid volunteerProfileId, Guid eventId, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetMyCertificatesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
    Task<int> GetActiveCertificateCountAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);

    // Badges
    Task<List<Badge>> GetAllActiveBadgesAsync(CancellationToken cancellationToken = default);
    Task<Badge?> GetBadgeByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<VolunteerBadge?> GetVolunteerBadgeAsync(Guid volunteerProfileId, Guid badgeId, CancellationToken cancellationToken = default);
    Task<List<VolunteerBadge>> GetMyBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default);
    void AddVolunteerBadge(VolunteerBadge volunteerBadge);
}
