using TicketReservation.Application.Interfaces;

namespace TicketReservation.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events");

        group.MapGet("/", async (IEventService eventService, CancellationToken cancellationToken) =>
        {
            var events = await eventService.ListEventsAsync(cancellationToken);
            return Results.Ok(events);
        })
        .WithName("ListEvents")
        .Produces(StatusCodes.Status200OK);
    }
}
