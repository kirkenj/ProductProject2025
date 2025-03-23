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
                var openApiDocuments = await Task.WhenAll(_options.ServiceConfigs.Select(GetDocumentationForService));

                var nullStreamsCount = openApiDocuments.Count(c => c == null);
                if (nullStreamsCount > 0)
                {
                    _logger.LogWarning("Couldn't recieve openApi documents from {invalidRequestsCount} services", nullStreamsCount);
                }
                else
                {
                    _logger.LogInformation("All requests for openApi documents succeded");
                }

                var documentStreamsToHandle = openApiDocuments.Where(c => c != null);

                OpenApiStreamReader openApiReader = new();
                
                OpenApiDocument? resultDocument = null;

                foreach (var openApiDocument in documentStreamsToHandle)
                { 
                    OpenApiDocument openApiDoc = openApiReader.Read(openApiDocument, out var diagnostic);
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

                    CombineOpenApiDocuments(resultDocument, openApiDoc);
                }

                if (resultDocument == null)
                {
                    var message = "Couldn't aggregate openApi docs";
                    _logger.LogError(message);
                    WriteTextResponse(context, message, 500);
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

        private async Task<Stream?> GetDocumentationForService(ServiceConfig serviceConfig)
        {
            try
            {
                var uri = $"{serviceConfig.DownstreamScheme}://{serviceConfig.Host}:{serviceConfig.Port}/{serviceConfig.SwaggerUrl}";

                var result = await _httpClient.GetAsync(uri);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" with uri \"{serviceSwaggerUrl}\"", serviceConfig, uri);
                    return null;
                }

                return await result.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" exception: {ex}", serviceConfig, ex);
                return null;
            }
        }

        private static void WriteTextResponse(HttpContext context, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.WriteAsync(message);
            context.Response.ContentType = "text/html";
        }

        private static void CombineOpenApiDocuments(OpenApiDocument destinationDocument, OpenApiDocument sourceDocument) 
        {

            foreach (var path in sourceDocument.Paths)
            {
                destinationDocument.Paths.Add(path.Key, path.Value);
            }

            foreach (var ext in sourceDocument.Components.Extensions)
            {
                destinationDocument.Components.Extensions.Add(ext);
            }

            foreach (var schema in sourceDocument.Components.Examples)
            {
                destinationDocument.Components.Examples.Add(schema.Key, schema.Value);
            }

            foreach (var schema in sourceDocument.Components.Schemas)
            {
                destinationDocument.Components.Schemas.Add(schema.Key, schema.Value);
            }

            foreach (var schema in sourceDocument.Components.Responses)
            {
                destinationDocument.Components.Responses.Add(schema.Key, schema.Value);
            }

            foreach (var schema in sourceDocument.Components.RequestBodies)
            {
                destinationDocument.Components.RequestBodies.Add(schema.Key, schema.Value);
            }
        }
    }
}
