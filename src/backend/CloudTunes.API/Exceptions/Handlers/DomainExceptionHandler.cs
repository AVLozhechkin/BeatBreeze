using System.Security.Authentication;
using CloudTunes.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CloudTunes.API.Exceptions.Handlers;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(ILogger<DomainExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        switch (exception)
        {
            case AlreadyExistException foundException:
                await HandleAlreadyExistException(httpContext, foundException, cancellationToken);
                break;
            case NotFoundException foundException:
                await HandleNotFoundException(httpContext, foundException, cancellationToken);
                break;
            case NotTheOwnerException foundException:
                await HandleNotTheOwnerException(httpContext, foundException, cancellationToken);
                break;
            case ValidationException foundException:
                await HandleValidationException(httpContext, foundException, cancellationToken);
                break;
            case AuthenticationException:
                await HandleAuthenticationException(httpContext, cancellationToken);
                break;
            default:
                return false;
        }

        _logger.LogError(exception, "Domain exception occured: {ExceptionMessage}", exception.Message);

        return true;
    }

    private static async Task HandleAlreadyExistException(HttpContext httpContext, AlreadyExistException ex, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Already exists",
            Detail = $"{ex.ResourceName} already exists."
        }, cancellationToken);
    }

    private static async Task HandleAuthenticationException(HttpContext httpContext, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Unauthorized",
            Detail = "The user email or password is incorrect."
        }, cancellationToken);
    }

    private static async Task HandleNotFoundException(HttpContext httpContext, NotFoundException ex, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        var message = ex.ResourseId is null ? $"{ex.ResourceName} was not found" :
            $"{ex.ResourceName} with ID: {ex.ResourseId} was not found";

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not found",
            Detail = message
        }, cancellationToken);
    }

    // Should be the same behavior as NotFound exception
    private static async Task HandleNotTheOwnerException(HttpContext httpContext, NotTheOwnerException ex, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        var message = ex.ResourseId is null ? $"{ex.ResourceName} was not found" :
            $"{ex.ResourceName} with ID: {ex.ResourseId} was not found";

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Not found",
            Detail = message
        }, cancellationToken);
    }

    private static async Task HandleValidationException(HttpContext httpContext, ValidationException foundException, CancellationToken cancellationToken)
    {
        // TODO Rework validation exceptions to include errors
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation error",
            Detail = foundException.Message
        }, cancellationToken);
    }
}
