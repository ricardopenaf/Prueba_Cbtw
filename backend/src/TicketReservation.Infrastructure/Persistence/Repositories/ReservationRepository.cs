using Microsoft.EntityFrameworkCore;
using TicketReservation.Application.Dtos;
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

    public async Task<IReadOnlyList<ReservationListItemResponse>> ListByDateRangeAsync(
        DateTime fromUtc,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken = default) =>
        await (from reservation in _dbContext.Reservations.AsNoTracking()
               join @event in _dbContext.Events.AsNoTracking() on reservation.EventId equals @event.Id
               join user in _dbContext.Users.AsNoTracking() on reservation.UserId equals user.Id
               where reservation.ReservedAtUtc >= fromUtc && reservation.ReservedAtUtc < toUtcExclusive
               orderby reservation.ReservedAtUtc
               select new ReservationListItemResponse(
                   reservation.Id,
                   @event.Code,
                   @event.Name,
                   user.Code,
                   user.FullName,
                   reservation.Quantity,
                   reservation.ReservedAtUtc))
            .ToListAsync(cancellationToken);
}
