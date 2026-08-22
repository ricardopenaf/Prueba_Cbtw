using TicketReservation.Application.Dtos;
using TicketReservation.Application.Interfaces;

namespace TicketReservation.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IReadOnlyList<EventSummaryResponse>> ListEventsAsync(CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.ListAsync(cancellationToken);

        return events
            .Select(e => new EventSummaryResponse(e.Code, e.Name, e.Venue, e.EventDateUtc, e.TotalCapacity, e.AvailableSeats))
            .ToList();
    }
}
