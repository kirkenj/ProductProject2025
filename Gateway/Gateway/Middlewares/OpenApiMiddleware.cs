using System.Text;
using Gateway.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;

namespace Gateway.Middlewares
{
    public class OpenApiMiddleware : IMiddleware
    {
        private readonly ServicesSettings _options;
        private readonly ILogger<OpenApiMiddleware> _logger;
        private readonly HttpClient _httpClient;

        public OpenApiMiddleware(IOptions<ServicesSettings> options, HttpClient httpClient, ILogger<OpenApiMiddleware> logger)
        {
            if (options.Value == null || options.Value.ServiceConfigs == null || options.Value.ServiceConfigs.Count == 0)
            {
                throw new ArgumentException($"{nameof(options)} can not be null or empty");
            }

            _logger = logger;

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _options = options.Value;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method == "GET" 
                && context.Request.Path.HasValue 
                && context.Request.Path.Value == _options.OpenApiPathPrefixSegment)
            {
                OpenApiDocument? resultDocument = null;

                foreach (var serviceConfig in _options.ServiceConfigs)
                {
                    var res = await GetDocumentationForService(serviceConfig);

                    if (string.IsNullOrEmpty(res))
                    {
                        continue;
                    }

                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(res));

                    OpenApiStreamReader openApiReader = new();

                    OpenApiDocument openApiDoc = openApiReader.Read(stream, out var diagnostic);

                    if (diagnostic.Errors.Count > 0)
                    {
                        var concatedErrors = string.Join("\r\n", diagnostic.Errors.Select(e => e.Message));
                        _logger.LogError("Errors found in the OpenAPI document: {errors}", concatedErrors);
                        continue;
                    }

                    if (resultDocument == null)
                    {
                        resultDocument = openApiDoc;
                        continue;
                    }

                    foreach (var path in openApiDoc.Paths)
                    {
                        resultDocument.Paths.Add(path.Key, path.Value);
                    }

                    foreach (var ext  in openApiDoc.Components.Extensions)
                    {
                        resultDocument.Components.Extensions.Add(ext);
                    }

                    foreach (var schema in openApiDoc.Components.Examples)
                    {
                        resultDocument.Components.Examples.Add(schema.Key, schema.Value);
                    }

                    foreach (var schema in openApiDoc.Components.Schemas)
                    {
                        resultDocument.Components.Schemas.Add(schema.Key, schema.Value);
                    }

                    foreach (var schema in openApiDoc.Components.Responses)
                    {
                        resultDocument.Components.Responses.Add(schema.Key, schema.Value);
                    }

                    foreach (var schema in openApiDoc.Components.RequestBodies)
                    {
                        resultDocument.Components.RequestBodies.Add(schema.Key, schema.Value);
                    }
                }

                if (resultDocument == null)
                {
                    _logger.LogError("Couldn't aggregate openApi docs");
                    WriteTextResponse(context, "Couldn't aggregate openApi docs", 500);
                    return;
                }

                resultDocument.Info.Title = _options.GatewayOpenApiDocumentName;

                using var stringWriter = new StringWriter();
                var openApiWriter = new OpenApiJsonWriter(stringWriter);
                resultDocument.SerializeAsV2(openApiWriter);

                string openApiString = stringWriter.ToString();

                var bytes = Encoding.UTF8.GetBytes(openApiString);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.BodyWriter.WriteAsync(bytes);
            }
            else
            {
                await next.Invoke(context);
            }
        }

        private async Task<string> GetDocumentationForService(ServiceConfig serviceConfig)
        {
            try
            {
                var uri = $"{serviceConfig.DownstreamScheme}://{serviceConfig.Host}:{serviceConfig.Port}/{serviceConfig.SwaggerUrl}";

                var result = await _httpClient.GetAsync(uri);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" with uri \"{serviceSwaggerUrl}\"", serviceConfig, uri);
                    return string.Empty;
                }

                var str = await result.Content.ReadAsStringAsync();

                return str;
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" exception: {ex}", serviceConfig, ex);
                return string.Empty;
            }
        }

        private static void WriteTextResponse(HttpContext context, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.WriteAsync(message);
            context.Response.ContentType = "text/html";
        }
    }
}
