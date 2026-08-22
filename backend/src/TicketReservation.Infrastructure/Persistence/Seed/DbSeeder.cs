using TicketReservation.Domain.Entities;

namespace TicketReservation.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!dbContext.Events.Any())
        {
            dbContext.Events.AddRange(
                new Event
                {
                    Id = Guid.NewGuid(),
                    Code = "EVT-001",
                    Name = "Concierto de Rock en vivo",
                    Venue = "Auditorio Nacional",
                    EventDateUtc = DateTime.UtcNow.AddDays(30),
                    TotalCapacity = 100,
                    AvailableSeats = 100,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Code = "EVT-002",
                    Name = "Conferencia de Tecnología",
                    Venue = "Centro de Convenciones",
                    EventDateUtc = DateTime.UtcNow.AddDays(45),
                    TotalCapacity = 50,
                    AvailableSeats = 50,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Code = "EVT-003",
                    Name = "Obra de Teatro Clásico",
                    Venue = "Teatro Municipal",
                    EventDateUtc = DateTime.UtcNow.AddDays(15),
                    TotalCapacity = 5,
                    AvailableSeats = 5,
                });
        }

        if (!dbContext.Users.Any())
        {
            dbContext.Users.AddRange(
                new User { Id = Guid.NewGuid(), Code = "USR-001", FullName = "Ana García", Email = "ana.garcia@example.com" },
                new User { Id = Guid.NewGuid(), Code = "USR-002", FullName = "Luis Martínez", Email = "luis.martinez@example.com" },
                new User { Id = Guid.NewGuid(), Code = "USR-003", FullName = "Carla Rodríguez", Email = "carla.rodriguez@example.com" });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
