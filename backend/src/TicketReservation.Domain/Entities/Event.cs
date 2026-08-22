namespace TicketReservation.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime EventDateUtc { get; set; }
    public int TotalCapacity { get; set; }
    public int AvailableSeats { get; set; }
}
