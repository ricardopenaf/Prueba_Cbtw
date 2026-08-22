namespace TicketReservation.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
