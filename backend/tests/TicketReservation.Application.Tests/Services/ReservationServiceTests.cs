using FluentAssertions;
using Moq;
using TicketReservation.Application.Dtos;
using TicketReservation.Application.Interfaces;
using TicketReservation.Application.Services;
using TicketReservation.Domain.Entities;
using TicketReservation.Domain.Exceptions;
using Xunit;

namespace TicketReservation.Application.Tests.Services;

public class ReservationServiceTests
{
    private readonly Mock<IEventRepository> _eventRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IReservationRepository> _reservationRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ReservationService _sut;

    public ReservationServiceTests()
    {
        _sut = new ReservationService(
            _eventRepository.Object,
            _userRepository.Object,
            _reservationRepository.Object,
            _unitOfWork.Object);

        // ExecuteInTransactionAsync simplemente invoca el delegado, igual que la implementación real
        // haría contra una base de datos real, pero sin abrir una transacción de verdad.
        _unitOfWork
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task<ReservationResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<ReservationResponse>> operation, CancellationToken ct) => operation(ct));
    }

    private static Event CreateEvent(string code = "EVT-001", int availableSeats = 10) => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        Name = "Evento de prueba",
        Venue = "Recinto de prueba",
        EventDateUtc = DateTime.UtcNow.AddDays(10),
        TotalCapacity = 100,
        AvailableSeats = availableSeats,
    };

    private static User CreateUser(string code = "USR-001") => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        FullName = "Usuario de prueba",
        Email = "usuario@example.com",
    };

    [Theory]
    [InlineData(null, "USR-001")]
    [InlineData("", "USR-001")]
    [InlineData(" ", "USR-001")]
    [InlineData("EVT-001", null)]
    [InlineData("EVT-001", "")]
    [InlineData("EVT-001", " ")]
    public async Task ReserveAsync_WithMissingCodes_ThrowsInvalidReservationRequestException(string? eventCode, string? userCode)
    {
        var request = new ReserveTicketRequest(eventCode!, userCode!, 1);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<InvalidReservationRequestException>();
        _eventRepository.Verify(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ReserveAsync_WithNonPositiveQuantity_ThrowsInvalidReservationRequestException(int quantity)
    {
        var request = new ReserveTicketRequest("EVT-001", "USR-001", quantity);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<InvalidReservationRequestException>();
    }

    [Fact]
    public async Task ReserveAsync_WithUnknownEventCode_ThrowsEventNotFoundException()
    {
        _eventRepository
            .Setup(r => r.GetByCodeAsync("EVT-404", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);

        var request = new ReserveTicketRequest("EVT-404", "USR-001", 1);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<EventNotFoundException>();
        _userRepository.Verify(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReserveAsync_WithUnknownUserCode_ThrowsUserNotFoundException()
    {
        _eventRepository
            .Setup(r => r.GetByCodeAsync("EVT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateEvent());
        _userRepository
            .Setup(r => r.GetByCodeAsync("USR-404", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new ReserveTicketRequest("EVT-001", "USR-404", 1);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<UserNotFoundException>();
        _reservationRepository.Verify(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReserveAsync_WhenUserAlreadyReservedEvent_ThrowsDuplicateReservationException()
    {
        var @event = CreateEvent();
        var user = CreateUser();

        _eventRepository.Setup(r => r.GetByCodeAsync(@event.Code, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
        _userRepository.Setup(r => r.GetByCodeAsync(user.Code, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _reservationRepository.Setup(r => r.ExistsAsync(@event.Id, user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var request = new ReserveTicketRequest(@event.Code, user.Code, 1);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<DuplicateReservationException>();
        _eventRepository.Verify(r => r.TryReserveSeatsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReserveAsync_WhenNotEnoughSeatsAvailable_ThrowsInsufficientCapacityException()
    {
        var @event = CreateEvent(availableSeats: 1);
        var user = CreateUser();

        _eventRepository.Setup(r => r.GetByCodeAsync(@event.Code, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
        _userRepository.Setup(r => r.GetByCodeAsync(user.Code, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _reservationRepository.Setup(r => r.ExistsAsync(@event.Id, user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _eventRepository
            .Setup(r => r.TryReserveSeatsAsync(@event.Id, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        var request = new ReserveTicketRequest(@event.Code, user.Code, 5);

        var act = () => _sut.ReserveAsync(request);

        await act.Should().ThrowAsync<InsufficientCapacityException>();
        _reservationRepository.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReserveAsync_WithValidRequest_CreatesReservationAndReturnsResponse()
    {
        var @event = CreateEvent(availableSeats: 10);
        var user = CreateUser();
        const int quantity = 2;
        const int remainingSeats = 8;

        _eventRepository.Setup(r => r.GetByCodeAsync(@event.Code, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
        _userRepository.Setup(r => r.GetByCodeAsync(user.Code, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _reservationRepository.Setup(r => r.ExistsAsync(@event.Id, user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _eventRepository
            .Setup(r => r.TryReserveSeatsAsync(@event.Id, quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(remainingSeats);

        var request = new ReserveTicketRequest(@event.Code, user.Code, quantity);

        var response = await _sut.ReserveAsync(request);

        response.EventCode.Should().Be(@event.Code);
        response.EventName.Should().Be(@event.Name);
        response.UserCode.Should().Be(user.Code);
        response.Quantity.Should().Be(quantity);
        response.RemainingSeats.Should().Be(remainingSeats);
        response.ReservationId.Should().NotBeEmpty();

        _reservationRepository.Verify(
            r => r.AddAsync(
                It.Is<Reservation>(res => res.EventId == @event.Id && res.UserId == user.Id && res.Quantity == quantity),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveAsync_WithValidRequest_ExecutesBusinessLogicWithinTransaction()
    {
        var @event = CreateEvent();
        var user = CreateUser();

        _eventRepository.Setup(r => r.GetByCodeAsync(@event.Code, It.IsAny<CancellationToken>())).ReturnsAsync(@event);
        _userRepository.Setup(r => r.GetByCodeAsync(user.Code, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _reservationRepository.Setup(r => r.ExistsAsync(@event.Id, user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _eventRepository.Setup(r => r.TryReserveSeatsAsync(@event.Id, 1, It.IsAny<CancellationToken>())).ReturnsAsync(9);

        var request = new ReserveTicketRequest(@event.Code, user.Code, 1);

        await _sut.ReserveAsync(request);

        _unitOfWork.Verify(
            u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task<ReservationResponse>>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
