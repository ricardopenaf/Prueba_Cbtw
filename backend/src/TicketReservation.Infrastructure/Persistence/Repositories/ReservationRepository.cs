using Microsoft.EntityFrameworkCore;
using TicketReservation.Application.Interfaces;
using TicketReservation.Domain.Entities;

namespace TicketReservation.Infrastructure.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _dbContext;

    public ReservationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default) =>
        _dbContext.Reservations.AsNoTracking().AnyAsync(r => r.EventId == eventId && r.UserId == userId, cancellationToken);

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default) =>
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
}
