using TicketReservation.Application.Dtos;
using TicketReservation.Application.Interfaces;

namespace TicketReservation.Api.Endpoints;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reservations").WithTags("Reservations");

        group.MapPost("/", async (ReserveTicketRequest request, IReservationService reservationService, CancellationToken cancellationToken) =>
        {
            var response = await reservationService.ReserveAsync(request, cancellationToken);
            return Results.Created($"/api/reservations/{response.ReservationId}", response);
        })
        .WithName("ReserveTicket")
        .Produces<ReservationResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
