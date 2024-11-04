using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Homework.Api.Exceptions;

namespace Homework.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ServiceUnavailableException ex)
            {
                _logger.LogError(ex, "External service is unavailable");
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable");
            }
            catch (JsonParseException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Bad Request: Invalid JSON format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred");
            }
        }
    }
}
