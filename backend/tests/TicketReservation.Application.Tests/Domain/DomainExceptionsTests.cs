using FluentAssertions;
using TicketReservation.Domain.Exceptions;
using Xunit;

namespace TicketReservation.Application.Tests.Domain;

public class DomainExceptionsTests
{
    [Fact]
    public void EventNotFoundException_SetsStatusCode404AndMessageWithEventCode()
    {
        var exception = new EventNotFoundException("EVT-404");

        exception.StatusCode.Should().Be(404);
        exception.Message.Should().Contain("EVT-404");
    }

    [Fact]
    public void UserNotFoundException_SetsStatusCode404AndMessageWithUserCode()
    {
        var exception = new UserNotFoundException("USR-404");

        exception.StatusCode.Should().Be(404);
        exception.Message.Should().Contain("USR-404");
    }

    [Fact]
    public void DuplicateReservationException_SetsStatusCode409AndMessageWithBothCodes()
    {
        var exception = new DuplicateReservationException("EVT-001", "USR-001");

        exception.StatusCode.Should().Be(409);
        exception.Message.Should().Contain("EVT-001");
        exception.Message.Should().Contain("USR-001");
    }

    [Fact]
    public void InsufficientCapacityException_SetsStatusCode409AndMessageWithRequestedAndAvailableSeats()
    {
        var exception = new InsufficientCapacityException("EVT-001", requestedQuantity: 5, availableSeats: 2);

        exception.StatusCode.Should().Be(409);
        exception.Message.Should().Contain("5");
        exception.Message.Should().Contain("2");
    }

    [Fact]
    public void InvalidReservationRequestException_SetsStatusCode400AndGivenMessage()
    {
        var exception = new InvalidReservationRequestException("La cantidad debe ser mayor que cero.");

        exception.StatusCode.Should().Be(400);
        exception.Message.Should().Be("La cantidad debe ser mayor que cero.");
    }
}
