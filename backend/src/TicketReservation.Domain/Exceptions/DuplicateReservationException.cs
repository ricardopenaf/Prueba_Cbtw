namespace TicketReservation.Domain.Exceptions;

public class DuplicateReservationException : DomainException
{
    public DuplicateReservationException(string eventCode, string userCode)
        : base($"El usuario '{userCode}' ya tiene una reserva para el evento '{eventCode}'.", 409)
    {
    }
}
