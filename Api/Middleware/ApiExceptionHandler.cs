using System.Net.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace Weatheria.Api.Middleware;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            HttpRequestException => (
                StatusCodes.Status503ServiceUnavailable,
                "External service unavailable",
                "The requested external service could not be reached."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "The request could not be completed.")
        };

        logger.LogError(
            exception,
            "Request {Method} {Path} failed with status code {StatusCode}.",
            httpContext.Request.Method,
            httpContext.Request.Path,
            statusCode);

        await Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: httpContext.Request.Path)
            .ExecuteAsync(httpContext);

        return true;
    }
}