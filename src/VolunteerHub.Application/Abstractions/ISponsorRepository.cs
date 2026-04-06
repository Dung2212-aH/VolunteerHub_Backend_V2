using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface ISponsorRepository
{
    Task<SponsorProfile?> GetSponsorProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SponsorProfile?> GetSponsorProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<bool> SponsorProfileExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<SponsorProfile>> GetPendingSponsorProfilesAsync(CancellationToken cancellationToken = default);

    Task<Event?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<SponsorshipPackage?> GetPackageByIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<List<SponsorshipPackage>> GetPackagesByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<EventSponsor?> GetEventSponsorByIdAsync(Guid eventSponsorId, CancellationToken cancellationToken = default);
    Task<List<EventSponsor>> GetEventSponsorsBySponsorProfileIdAsync(Guid sponsorProfileId, CancellationToken cancellationToken = default);
    Task<List<EventSponsor>> GetEventSponsorsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveEventSponsorAsync(Guid sponsorProfileId, Guid eventId, CancellationToken cancellationToken = default);

    void AddSponsorProfile(SponsorProfile profile);
    void UpdateSponsorProfile(SponsorProfile profile);

    void AddSponsorshipPackage(SponsorshipPackage package);
    void UpdateSponsorshipPackage(SponsorshipPackage package);

    void AddEventSponsor(EventSponsor eventSponsor);
    void UpdateEventSponsor(EventSponsor eventSponsor);

    void AddContribution(SponsorContribution contribution);
}