using FluentAssertions;
using Moq;
using TicketReservation.Application.Interfaces;
using TicketReservation.Application.Services;
using TicketReservation.Domain.Entities;
using Xunit;

namespace TicketReservation.Application.Tests.Services;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepository = new();
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _sut = new EventService(_eventRepository.Object);
    }

    [Fact]
    public async Task ListEventsAsync_WithNoEvents_ReturnsEmptyList()
    {
        _eventRepository
            .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Event>());

        var result = await _sut.ListEventsAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListEventsAsync_WithEvents_ReturnsMappedSummariesPreservingRepositoryOrder()
    {
        var events = new[]
        {
            new Event
            {
                Id = Guid.NewGuid(),
                Code = "EVT-003",
                Name = "Obra de Teatro Clásico",
                Venue = "Teatro Municipal",
                EventDateUtc = DateTime.UtcNow.AddDays(15),
                TotalCapacity = 5,
                AvailableSeats = 5,
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Code = "EVT-001",
                Name = "Concierto de Rock en vivo",
                Venue = "Auditorio Nacional",
                EventDateUtc = DateTime.UtcNow.AddDays(30),
                TotalCapacity = 100,
                AvailableSeats = 97,
            },
        };

        _eventRepository
            .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        var result = await _sut.ListEventsAsync();

        result.Should().HaveCount(2);
        result.Select(r => r.Code).Should().ContainInOrder("EVT-003", "EVT-001");

        var first = result.First();
        first.Name.Should().Be("Obra de Teatro Clásico");
        first.Venue.Should().Be("Teatro Municipal");
        first.TotalCapacity.Should().Be(5);
        first.AvailableSeats.Should().Be(5);
    }
}
