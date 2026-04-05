using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class RecognitionRepository : IRecognitionRepository
{
    private readonly AppDbContext _context;

    public RecognitionRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddCertificate(Certificate certificate) => _context.Certificates.Add(certificate);

    public void UpdateCertificate(Certificate certificate) => _context.Certificates.Update(certificate);

    public async Task<Certificate?> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Event)
            .Include(c => c.VolunteerProfile)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Certificate?> GetCertificateByVerificationCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Event)
            .Include(c => c.VolunteerProfile)
            .FirstOrDefaultAsync(c => c.VerificationCode == code, cancellationToken);
    }

    public async Task<Certificate?> GetActiveCertificateAsync(Guid volunteerProfileId, Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(c => c.VolunteerProfileId == volunteerProfileId 
                                   && c.EventId == eventId 
                                   && c.Status == CertificateStatus.Active, cancellationToken);
    }

    public async Task<List<Certificate>> GetMyCertificatesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Event)
            .Where(c => c.VolunteerProfileId == volunteerProfileId)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveCertificateCountAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .CountAsync(c => c.VolunteerProfileId == volunteerProfileId && c.Status == CertificateStatus.Active, cancellationToken);
    }

    public async Task<List<Badge>> GetAllActiveBadgesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Badges.Where(b => b.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<Badge?> GetBadgeByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Badges.FirstOrDefaultAsync(b => b.Code == code, cancellationToken);
    }

    public async Task<VolunteerBadge?> GetVolunteerBadgeAsync(Guid volunteerProfileId, Guid badgeId, CancellationToken cancellationToken = default)
    {
        return await _context.VolunteerBadges
            .FirstOrDefaultAsync(v => v.VolunteerProfileId == volunteerProfileId && v.BadgeId == badgeId, cancellationToken);
    }

    public async Task<List<VolunteerBadge>> GetMyBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.VolunteerBadges
            .Include(v => v.Badge)
            .Where(v => v.VolunteerProfileId == volunteerProfileId)
            .OrderByDescending(v => v.AwardedAt)
            .ToListAsync(cancellationToken);
    }

    public void AddVolunteerBadge(VolunteerBadge volunteerBadge) => _context.VolunteerBadges.Add(volunteerBadge);
}
