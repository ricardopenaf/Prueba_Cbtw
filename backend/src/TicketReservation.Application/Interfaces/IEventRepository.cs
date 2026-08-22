using TicketReservation.Domain.Entities;

namespace TicketReservation.Application.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrementa AvailableSeats de forma atómica en una sola sentencia UPDATE.
    /// Devuelve el aforo restante tras la reserva, o null si no había aforo suficiente.
    /// </summary>
    Task<int?> TryReserveSeatsAsync(Guid eventId, int quantity, CancellationToken cancellationToken = default);
}
