using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Abstractions;

public interface IEventRepository
{
    Task<Event?> GetDetailsByIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<List<Event>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken cancellationToken = default);
    void Add(Event ev);
    void Update(Event ev);
}
