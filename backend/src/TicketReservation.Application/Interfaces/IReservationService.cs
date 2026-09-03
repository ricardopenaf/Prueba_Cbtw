using TicketReservation.Application.Dtos;

namespace TicketReservation.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> ReserveAsync(ReserveTicketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista las reservas realizadas entre <paramref name="fromDate"/> y <paramref name="toDate"/>, ambas inclusive.
    /// </summary>
    Task<IReadOnlyList<ReservationListItemResponse>> ListByDateRangeAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}
