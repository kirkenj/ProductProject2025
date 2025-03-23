using FluentValidation;
using System.Text.Json;

namespace AuthAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException valEx)
            {
                context.Response.StatusCode = 400;
                string message = $"{nameof(ValidationException)}: " + JsonSerializer.Serialize(valEx.Errors);
                await context.Response.WriteAsync(message);
                return;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(_environment.IsDevelopment() ? ex.Message : "Ooopsie", typeof(string));
                _logger.Log(LogLevel.Critical, ex, message: ex.Message);
            }
        }
    }
}
