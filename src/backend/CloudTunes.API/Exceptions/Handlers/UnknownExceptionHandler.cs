using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CloudTunes.API.Exceptions.Handlers;

public sealed class UnknownExceptionHandler : IExceptionHandler
{
    private readonly ILogger<UnknownExceptionHandler> _logger;

    public UnknownExceptionHandler(ILogger<UnknownExceptionHandler> logger)
    {
        _logger = logger;
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        _logger.LogError(exception, "An exception occured during the request");

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = "Internal server error. Please, try again."
        }, cancellationToken);

        return true;
    }
}
