using TicketReservation.Domain.Entities;

namespace TicketReservation.Application.Interfaces;

public interface IReservationRepository
{
    Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
