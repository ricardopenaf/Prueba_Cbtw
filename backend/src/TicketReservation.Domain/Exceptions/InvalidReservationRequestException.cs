namespace TicketReservation.Domain.Exceptions;

public class InvalidReservationRequestException : DomainException
{
    public InvalidReservationRequestException(string message) : base(message, 400)
    {
    }
}
