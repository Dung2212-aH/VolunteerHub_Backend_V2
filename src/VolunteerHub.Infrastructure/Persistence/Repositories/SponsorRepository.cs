using Microsoft.EntityFrameworkCore;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Repositories;

public class SponsorRepository : ISponsorRepository
{
    private readonly AppDbContext _context;

    public SponsorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SponsorProfile?> GetSponsorProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorProfiles
            .Include(x => x.ContactPersons)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<SponsorProfile?> GetSponsorProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorProfiles
            .Include(x => x.ContactPersons)
            .FirstOrDefaultAsync(x => x.Id == profileId, cancellationToken);
    }

    public async Task<bool> SponsorProfileExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorProfiles.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<List<SponsorProfile>> GetPendingSponsorProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SponsorProfiles
            .Include(x => x.ContactPersons)
            .Where(x => x.Status == SponsorProfileStatus.PendingApproval)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(x => x.SponsorshipPackages)
            .Include(x => x.EventSponsors)
            .FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);
    }

    public async Task<SponsorshipPackage?> GetPackageByIdAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorshipPackages
            .FirstOrDefaultAsync(x => x.Id == packageId, cancellationToken);
    }

    public async Task<List<SponsorshipPackage>> GetPackagesByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.SponsorshipPackages
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Amount)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventSponsor?> GetEventSponsorByIdAsync(Guid eventSponsorId, CancellationToken cancellationToken = default)
    {
        return await _context.EventSponsors
            .Include(x => x.SponsorProfile)
            .Include(x => x.Event)
            .Include(x => x.SponsorshipPackage)
            .Include(x => x.Contributions)
            .FirstOrDefaultAsync(x => x.Id == eventSponsorId, cancellationToken);
    }

    public async Task<List<EventSponsor>> GetEventSponsorsBySponsorProfileIdAsync(Guid sponsorProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.EventSponsors
            .Include(x => x.SponsorProfile)
            .Include(x => x.Event)
            .Include(x => x.SponsorshipPackage)
            .Where(x => x.SponsorProfileId == sponsorProfileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EventSponsor>> GetEventSponsorsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.EventSponsors
            .Include(x => x.SponsorProfile)
            .Include(x => x.Event)
            .Include(x => x.SponsorshipPackage)
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.DisplayPriority)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveEventSponsorAsync(Guid sponsorProfileId, Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.EventSponsors.AnyAsync(
            x => x.SponsorProfileId == sponsorProfileId
              && x.EventId == eventId
              && x.Status != EventSponsorStatus.Rejected
              && x.Status != EventSponsorStatus.Withdrawn,
            cancellationToken);
    }

    public void AddSponsorProfile(SponsorProfile profile)
    {
        _context.SponsorProfiles.Add(profile);
    }

    public void UpdateSponsorProfile(SponsorProfile profile)
    {
        _context.SponsorProfiles.Update(profile);
    }

    public void AddSponsorshipPackage(SponsorshipPackage package)
    {
        _context.SponsorshipPackages.Add(package);
    }

    public void UpdateSponsorshipPackage(SponsorshipPackage package)
    {
        _context.SponsorshipPackages.Update(package);
    }

    public void AddEventSponsor(EventSponsor eventSponsor)
    {
        _context.EventSponsors.Add(eventSponsor);
    }

    public void UpdateEventSponsor(EventSponsor eventSponsor)
    {
        _context.EventSponsors.Update(eventSponsor);
    }

    public void AddContribution(SponsorContribution contribution)
    {
        _context.SponsorContributions.Add(contribution);
    }
}