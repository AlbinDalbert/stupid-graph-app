using kiss_graph_api.DTOs;
using kiss_graph_api.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Text.Json;

// Use your project's namespace and the new folder
namespace kiss_graph_api.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;


        public GlobalExceptionHandlerMiddleware(RequestDelegate next,
                                                ILogger<GlobalExceptionHandlerMiddleware> logger,
                                                IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException nex)
            {         
                _logger.LogWarning("Resource not found for {Method} {Path}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    nex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                var response = new ErrorResponseDto
                {
                    StatusCode = context.Response.StatusCode,
                    Message = nex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--- UNHANDLED EXCEPTION --- Request: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var responseMessage = new ErrorResponseDto
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An internal server error occurred. Please try again later."
                };

                if (_env.IsDevelopment())
                {
                    responseMessage.Detail = ex.ToString();
                }
                // --- End DTO Usage ---

                var jsonResponse = JsonSerializer.Serialize(responseMessage);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}