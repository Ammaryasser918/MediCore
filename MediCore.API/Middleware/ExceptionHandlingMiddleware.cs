using MediCore.Application.Common.Exceptions;
using MediCore.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace MediCore.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                DuplicateEntityException => HttpStatusCode.Conflict,
                AuthenticationRequiredException => HttpStatusCode.Unauthorized,
                ForbiddenAccessException => HttpStatusCode.Forbidden,
                BusinessRuleViolationException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = JsonSerializer.Serialize(new
            {
                status = (int)statusCode,
                error = statusCode.ToString(),
                message = exception.Message
            });

            return context.Response.WriteAsync(payload);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
