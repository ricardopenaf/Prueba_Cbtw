using Microsoft.EntityFrameworkCore;
using TicketReservation.Application.Interfaces;
using TicketReservation.Domain.Entities;

namespace TicketReservation.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Code == code, cancellationToken);
}
