using System.Text;
using Gateway.Models;
using Gateway.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Gateway.Controllers
{
    public class OpenApiMiddleware : IMiddleware
    {
        private readonly IOpenApiService _openApiService;
        private readonly OpenApiServiceSettings _options;

        public OpenApiMiddleware(IOpenApiService openApiService, IOptions<OpenApiServiceSettings> options)
        {
            _openApiService = openApiService ?? throw new ArgumentNullException(nameof(openApiService));
            _options = options.Value;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments($"/{_options.OpenApiPathPrefixSegment}"))
            {
                var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (segments == null || segments.Length != 2)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                var res = await _openApiService.GetDocumentationForService(segments[1]);
                var bytes = Encoding.UTF8.GetBytes(res);


                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.BodyWriter.WriteAsync(bytes);
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
