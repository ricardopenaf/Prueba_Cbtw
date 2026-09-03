namespace TicketReservation.Application.Dtos;

public record ReservationListItemResponse(
    Guid ReservationId,
    string EventCode,
    string EventName,
    string UserCode,
    string UserFullName,
    int Quantity,
    DateTime ReservedAtUtc);
