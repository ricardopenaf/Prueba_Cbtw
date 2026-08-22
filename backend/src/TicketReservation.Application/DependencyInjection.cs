using Microsoft.Extensions.DependencyInjection;
using TicketReservation.Application.Interfaces;
using TicketReservation.Application.Services;

namespace TicketReservation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}
