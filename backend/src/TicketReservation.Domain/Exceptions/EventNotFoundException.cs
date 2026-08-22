namespace TicketReservation.Domain.Exceptions;

public class EventNotFoundException : DomainException
{
    public EventNotFoundException(string eventCode)
        : base($"No existe ningún evento con el código '{eventCode}'.", 404)
    {
    }
}
