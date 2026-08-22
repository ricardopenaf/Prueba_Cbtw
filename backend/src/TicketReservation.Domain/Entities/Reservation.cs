namespace TicketReservation.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public DateTime ReservedAtUtc { get; set; }
}
