using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class VolunteerProfileRepository : IVolunteerProfileRepository
{
    private readonly AppDbContext _context;

    public VolunteerProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VolunteerProfile?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.VolunteerProfiles
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<VolunteerProfile?> GetByIdWithDetailsAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _context.VolunteerProfiles
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.VolunteerProfiles.AnyAsync(p => p.UserId == userId, cancellationToken);
    }

    public void Add(VolunteerProfile profile)
    {
        _context.VolunteerProfiles.Add(profile);
    }

    public void Update(VolunteerProfile profile)
    {
        _context.VolunteerProfiles.Update(profile);
    }
}
