namespace TicketReservation.Application.Dtos;

public record ReserveTicketRequest(string EventCode, string UserCode, int Quantity);
