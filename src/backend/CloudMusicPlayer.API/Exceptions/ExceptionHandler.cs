using System.Security.Authentication;
using CloudMusicPlayer.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Exceptions;

public sealed class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case NotFoundException foundException:
                await HandleNotFoundException(ctx, foundException);
                break;
            case NotTheOwnerException foundException:
                await HandleNotTheOwnerException(ctx, foundException);
                break;
            case ValidationException:
                await HandleValidationException(ctx, exception);
                break;
            case ExternalApiException:
                // Should return 500
                await HandleUnknownException(ctx);
                break;
            case AuthenticationException:
                await HandleAuthenticationException(ctx);
                break;
            default:
                await HandleUnknownException(ctx);
                break;
        }

        return true;
    }

    private static async Task HandleAuthenticationException(HttpContext ctx)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await ctx.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = "The user email or password is incorrect."
        });
    }

    private static async Task HandleNotFoundException(HttpContext ctx, NotFoundException ex)
    {
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;

        var message = ex.ResourseId is null ? $"{ex.ResourceName} was not found" :
            $"{ex.ResourceName} with ID: {ex.ResourseId} was not found";

        await ctx.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = message
        });
    }

    // Should be the same behavior as NotFound exception
    private static async Task HandleNotTheOwnerException(HttpContext ctx, NotTheOwnerException ex)
    {
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;

        var message = ex.ResourseId is null ? $"{ex.ResourceName} was not found" :
            $"{ex.ResourceName} with ID: {ex.ResourseId} was not found";

        await ctx.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = message
        });
    }

    private static async Task HandleValidationException(HttpContext ctx, Exception foundException)
    {
        // TODO Rework validation exceptions to include errors
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

        await ctx.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = foundException.Message
        });
    }

    private static async Task HandleUnknownException(HttpContext ctx)
    {
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await ctx.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = "Internal server error. Please, try again."
        });
    }
}
