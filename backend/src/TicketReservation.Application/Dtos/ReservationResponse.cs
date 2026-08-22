namespace TicketReservation.Application.Dtos;

public record ReservationResponse(
    Guid ReservationId,
    string EventCode,
    string EventName,
    string UserCode,
    int Quantity,
    DateTime ReservedAtUtc,
    int RemainingSeats);
