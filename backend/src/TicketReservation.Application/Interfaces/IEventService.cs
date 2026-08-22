using TicketReservation.Application.Dtos;

namespace TicketReservation.Application.Interfaces;

public interface IEventService
{
    Task<IReadOnlyList<EventSummaryResponse>> ListEventsAsync(CancellationToken cancellationToken = default);
}
