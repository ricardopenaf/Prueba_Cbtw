namespace TicketReservation.Application.Dtos;

public record EventSummaryResponse(
    string Code,
    string Name,
    string Venue,
    DateTime EventDateUtc,
    int TotalCapacity,
    int AvailableSeats);
