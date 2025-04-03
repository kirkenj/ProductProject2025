using FluentValidation;

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
                await context.Response.WriteAsJsonAsync(valEx.Errors);
                return;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(_environment.IsDevelopment() ? ex.Message : "Ooopsie", typeof(string));
                _logger.LogError(ex, string.Empty);
            }
        }
    }
}
