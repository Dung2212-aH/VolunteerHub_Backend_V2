using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class OrganizerRepository : IOrganizerRepository
{
    private readonly AppDbContext _context;

    public OrganizerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizerProfiles
            .Include(o => o.LegalDocuments)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<OrganizerProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizerProfiles
            .Include(o => o.LegalDocuments)
            .FirstOrDefaultAsync(o => o.Id == profileId, cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizerProfiles.AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<List<OrganizerProfile>> GetPendingProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrganizerProfiles
            .Where(o => o.VerificationStatus == OrganizerVerificationStatus.Pending || 
                        o.VerificationStatus == OrganizerVerificationStatus.UnderReview)
            .ToListAsync(cancellationToken);
    }

    public void Add(OrganizerProfile profile)
    {
        _context.OrganizerProfiles.Add(profile);
    }

    public void Update(OrganizerProfile profile)
    {
        _context.OrganizerProfiles.Update(profile);
    }

    public void AddReview(OrganizerVerificationReview review)
    {
        _context.OrganizerVerificationReviews.Add(review);
    }
}
