using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrganizerVerificationService _organizerVerificationService;

    public EventService(IEventRepository eventRepository, IUnitOfWork unitOfWork, IOrganizerVerificationService organizerVerificationService)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _organizerVerificationService = organizerVerificationService;
    }

    public async Task<Result<EventResponse>> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null)
            return Result.Failure<EventResponse>(Error.NotFound);

        return Result.Success(MapToResponse(ev));
    }

    public async Task<Result<List<EventResponse>>> GetEventsByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetByOrganizerIdAsync(organizerId, cancellationToken);
        return Result.Success(events.Select(MapToResponse).ToList());
    }

    public async Task<Result> CreateEventAsync(Guid organizerId, CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StartAt >= request.EndAt)
            return Result.Failure(new Error("Event.InvalidDates", "StartAt must be before EndAt."));

        var ev = new Event
        {
            OrganizerId = organizerId,
            Title = request.Title,
            Description = request.Description,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Capacity = request.Capacity,
            Status = EventStatus.Draft
        };

        UpdateSkills(ev, request.Skills);

        _eventRepository.Add(ev);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateEventAsync(Guid organizerId, Guid eventId, UpdateEventRequest request, bool isAdminOverride = false, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || (!isAdminOverride && ev.OrganizerId != organizerId))
            return Result.Failure(Error.NotFound);

        if (!isAdminOverride && ev.Status == EventStatus.Completed)
            return Result.Failure(new Error("Event.Immutable", "Completed events cannot be edited by organizers."));

        if (request.StartAt >= request.EndAt)
            return Result.Failure(new Error("Event.InvalidDates", "StartAt must be before EndAt."));

        ev.Title = request.Title;
        ev.Description = request.Description;
        ev.StartAt = request.StartAt;
        ev.EndAt = request.EndAt;
        ev.Address = request.Address;
        ev.Latitude = request.Latitude;
        ev.Longitude = request.Longitude;
        ev.Capacity = request.Capacity;

        UpdateSkills(ev, request.Skills);

        _eventRepository.Update(ev);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> PublishEventAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var isVerified = await _organizerVerificationService.IsOrganizerVerifiedAsync(organizerId, cancellationToken);
        if (!isVerified)
            return Result.Failure(new Error("Event.Unverified", "Only verified organizers can publish events."));
            
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId)
            return Result.Failure(Error.NotFound);

        if (ev.Status != EventStatus.Draft)
            return Result.Failure(new Error("Event.InvalidStatus", "Only Draft events can be published."));

        ev.Status = EventStatus.Published;
        _eventRepository.Update(ev);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CloseEventAsync(Guid organizerId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetDetailsByIdAsync(eventId, cancellationToken);
        if (ev == null || ev.OrganizerId != organizerId)
            return Result.Failure(Error.NotFound);

        // Only Published events can usually be closed prematurely
        if (ev.Status != EventStatus.Published)
            return Result.Failure(new Error("Event.InvalidStatus", "Only Published events can be closed."));

        ev.Status = EventStatus.Closed;
        _eventRepository.Update(ev);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private void UpdateSkills(Event ev, List<string> skills)
    {
        ev.SkillRequirements.Clear();
        foreach (var name in skills.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            ev.SkillRequirements.Add(new EventSkillRequirement { SkillName = name, EventId = ev.Id });
        }
    }

    private EventResponse MapToResponse(Event ev)
    {
        return new EventResponse
        {
            Id = ev.Id,
            OrganizerId = ev.OrganizerId,
            Title = ev.Title,
            Description = ev.Description,
            StartAt = ev.StartAt,
            EndAt = ev.EndAt,
            Address = ev.Address,
            Latitude = ev.Latitude,
            Longitude = ev.Longitude,
            Capacity = ev.Capacity,
            Status = ev.Status.ToString(),
            Skills = ev.SkillRequirements.Select(s => s.SkillName).ToList()
        };
    }
}
