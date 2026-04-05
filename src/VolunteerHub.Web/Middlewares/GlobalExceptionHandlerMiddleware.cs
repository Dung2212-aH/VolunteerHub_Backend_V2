using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VolunteerHub.Web.Middlewares;

/// <summary>
/// Global exception handler using the .NET 8+ IExceptionHandler pattern.
/// - For AJAX / API requests → returns JSON ProblemDetails.
/// - For MVC requests → falls through to the default UseExceptionHandler("/Home/Error").
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception — {ExceptionType}: {Message}",
            exception.GetType().Name,
            exception.Message);

        // Return JSON for AJAX / API callers
        var accept = httpContext.Request.Headers.Accept.ToString();
        if (accept.Contains("application/json") ||
            httpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }, cancellationToken);

            return true; // handled
        }

        // Let the default MVC error handler render the error view
        return false;
    }
}
