namespace TicketReservation.Domain.Exceptions;

public class InsufficientCapacityException : DomainException
{
    public InsufficientCapacityException(string eventCode, int requestedQuantity, int availableSeats)
        : base(
            $"El evento '{eventCode}' no tiene aforo suficiente. Solicitadas: {requestedQuantity}, disponibles: {availableSeats}.",
            409)
    {
    }
}
