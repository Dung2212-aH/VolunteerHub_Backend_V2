using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;

namespace VolunteerHub.Application.Abstractions;

public interface IEventService
{
    Task<Result<EventResponse>> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<Result<List<EventResponse>>> GetEventsByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default);
    Task<Result> CreateEventAsync(Guid organizerId, CreateEventRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateEventAsync(Guid organizerId, Guid eventId, UpdateEventRequest request, bool isAdminOverride = false, CancellationToken cancellationToken = default);
    Task<Result> PublishEventAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default);
    Task<Result> CloseEventAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default);
}
