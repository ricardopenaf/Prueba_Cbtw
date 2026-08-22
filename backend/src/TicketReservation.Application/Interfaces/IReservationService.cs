using TicketReservation.Application.Dtos;

namespace TicketReservation.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> ReserveAsync(ReserveTicketRequest request, CancellationToken cancellationToken = default);
}
