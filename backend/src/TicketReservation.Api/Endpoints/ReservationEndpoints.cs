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

        group.MapGet("/", async (DateOnly? from, DateOnly? to, IReservationService reservationService, CancellationToken cancellationToken) =>
        {
            if (from is null || to is null)
            {
                return Results.Problem(
                    detail: "Los parámetros 'from' y 'to' son obligatorios y deben tener el formato YYYY-MM-DD.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "InvalidDateRange");
            }

            var reservations = await reservationService.ListByDateRangeAsync(from.Value, to.Value, cancellationToken);
            return Results.Ok(reservations);
        })
        .WithName("ListReservationsByDateRange")
        .Produces<IReadOnlyList<ReservationListItemResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
