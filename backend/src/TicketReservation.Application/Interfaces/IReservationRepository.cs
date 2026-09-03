using TicketReservation.Application.Dtos;
using TicketReservation.Domain.Entities;

namespace TicketReservation.Application.Interfaces;

public interface IReservationRepository
{
    Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve las reservas cuyo <c>ReservedAtUtc</c> está en el rango [fromUtc, toUtcExclusive),
    /// junto con los datos del evento y del usuario, ordenadas por fecha de reserva ascendente.
    /// </summary>
    Task<IReadOnlyList<ReservationListItemResponse>> ListByDateRangeAsync(
        DateTime fromUtc,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken = default);
}
