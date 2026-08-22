namespace TicketReservation.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string userCode)
        : base($"No existe ningún usuario con el código '{userCode}'.", 404)
    {
    }
}
