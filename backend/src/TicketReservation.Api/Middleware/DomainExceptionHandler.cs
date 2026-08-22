using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TicketReservation.Domain.Exceptions;

namespace TicketReservation.Api.Middleware;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = domainException.StatusCode,
            Title = domainException.GetType().Name,
            Detail = domainException.Message,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.StatusCode = domainException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
