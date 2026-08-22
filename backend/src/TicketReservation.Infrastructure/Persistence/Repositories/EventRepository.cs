using Microsoft.EntityFrameworkCore;
using TicketReservation.Application.Interfaces;
using TicketReservation.Domain.Entities;

namespace TicketReservation.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _dbContext;

    public EventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Event?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Code == code, cancellationToken);

    public async Task<IReadOnlyList<Event>> ListAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Events.AsNoTracking().OrderBy(e => e.EventDateUtc).ToListAsync(cancellationToken);

    public async Task<int?> TryReserveSeatsAsync(Guid eventId, int quantity, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.Events
            .Where(e => e.Id == eventId && e.AvailableSeats >= quantity)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(e => e.AvailableSeats, e => e.AvailableSeats - quantity),
                cancellationToken);

        if (rowsAffected == 0)
        {
            return null;
        }

        var remainingSeats = await _dbContext.Events
            .Where(e => e.Id == eventId)
            .Select(e => e.AvailableSeats)
            .FirstAsync(cancellationToken);

        return remainingSeats;
    }
}
