using TicketReservation.Application.Dtos;
using TicketReservation.Application.Interfaces;
using TicketReservation.Domain.Entities;
using TicketReservation.Domain.Exceptions;


namespace TicketReservation.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationResponse> ReserveAsync(ReserveTicketRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.EventCode) || string.IsNullOrWhiteSpace(request.UserCode))
        {
            throw new InvalidReservationRequestException("El código de evento y el código de usuario son obligatorios.");
        }

        if (request.Quantity <= 0)
        {
            throw new InvalidReservationRequestException("La cantidad de entradas debe ser mayor que cero.");
        }

        var @event = await _eventRepository.GetByCodeAsync(request.EventCode, cancellationToken)
            ?? throw new EventNotFoundException(request.EventCode);

        var user = await _userRepository.GetByCodeAsync(request.UserCode, cancellationToken)
            ?? throw new UserNotFoundException(request.UserCode);

        return await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var alreadyReserved = await _reservationRepository.ExistsAsync(@event.Id, user.Id, ct);
            if (alreadyReserved)
            {
                throw new DuplicateReservationException(request.EventCode, request.UserCode);
            }

            var remainingSeats = await _eventRepository.TryReserveSeatsAsync(@event.Id, request.Quantity, ct);
            if (remainingSeats is null)
            {
                throw new InsufficientCapacityException(request.EventCode, request.Quantity, @event.AvailableSeats);
            }

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                EventId = @event.Id,
                UserId = user.Id,
                Quantity = request.Quantity,
                ReservedAtUtc = DateTime.UtcNow,
            };

            await _reservationRepository.AddAsync(reservation, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new ReservationResponse(
                reservation.Id,
                @event.Code,
                @event.Name,
                user.Code,
                reservation.Quantity,
                reservation.ReservedAtUtc,
                remainingSeats.Value);
        }, cancellationToken);
    }
}
