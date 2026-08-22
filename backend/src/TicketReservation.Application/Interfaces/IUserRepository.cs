using TicketReservation.Domain.Entities;

namespace TicketReservation.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
