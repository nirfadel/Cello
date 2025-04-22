using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Helper
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred while processing request {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }


        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ApplicationException appEx => appEx.Severity switch
                {
                    ErrorSeverity.Critical or ErrorSeverity.High => StatusCodes.Status500InternalServerError,
                    ErrorSeverity.Medium => StatusCodes.Status400BadRequest,
                    ErrorSeverity.Low => StatusCodes.Status200OK,
                    _ => StatusCodes.Status500InternalServerError
                },
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            // Return a generic error message in production, with more details in development
            string message = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? exception.Message
                : "An error occurred. Please try again later.";

            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = context.Response.StatusCode,
                message = message,
                requestId = Activity.Current?.Id ?? context.TraceIdentifier
            }));
        }
    }
}
