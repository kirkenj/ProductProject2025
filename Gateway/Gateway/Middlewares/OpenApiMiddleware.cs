using System.Text;
using System.Text.Json.Nodes;
using Gateway.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;

namespace Gateway.Middlewares
{
    public class OpenApiMiddleware : IMiddleware
    {
        private const string COMPONENTS_TAG = "components";
        private const string SCHEMAS_TAG = "schemas";
        private const string PATH_TAG = "paths";

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
                await HandleGetOpenApiDocumentRequestAsync(context);
            }
            else
            {
                await next.Invoke(context);
            }
        }

        private async Task HandleGetOpenApiDocumentRequestAsync(HttpContext context)
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

            string? resultDocument = null;

            foreach (var openApiDocument in documentStreamsToHandle)
            {
                if (resultDocument == null)
                {
                    resultDocument = openApiDocument;
                    continue;
                }

                resultDocument = CombineOpenApiDocuments(resultDocument, openApiDocument!, out ICollection<string> errors);
                if (errors.Count > 0)
                {
                    _logger.LogError(string.Join("\r\n", errors));
                }
            }

            if (resultDocument == null)
            {
                var message = "Couldn't aggregate openApi docs";
                _logger.LogError(message);
                await WriteResponseAsync(context, message, 500);
                return;
            }

            var reader = new OpenApiStringReader();

            var openApiDoc = reader.Read(resultDocument, out var diagnostic);
            if (diagnostic.Errors.Count > 0)
            {
                var concatedErrors = string.Join("\r\n", diagnostic.Errors.Select(e => e.Message));
                _logger.LogError("Errors found in the OpenAPI document: {errors}", concatedErrors);
            }

            openApiDoc.Info.Title = _options.GatewayOpenApiDocumentName;

            using var stringWriter = new StringWriter();
            var openApiWriter = new OpenApiJsonWriter(stringWriter);
            openApiDoc.SerializeAsV2(openApiWriter);

            string openApiString = stringWriter.ToString();

            await WriteResponseAsync(context, openApiString, 200, "application/json");
        }

        private static string CombineOpenApiDocuments(string firstDocument, string secondDocument, out ICollection<string> errors)
        {
            try
            {
                List<string> errorsList = new();
                var firstDocumentJson = JsonObject.Parse(firstDocument) ?? throw new Exception($"Couldn't parse jsonObject from {nameof(firstDocument)}");
                var secondDocumentJson = JsonObject.Parse(secondDocument) ?? throw new Exception($"Couldn't parse jsonObject from {nameof(secondDocument)}");

                var firstComponentsSection = firstDocumentJson[COMPONENTS_TAG]?[SCHEMAS_TAG]?.AsObject();
                var secondComponentsSection = secondDocumentJson[COMPONENTS_TAG]?[SCHEMAS_TAG]?.AsObject();

                if (firstComponentsSection == null)
                {
                    errorsList.Add($"Couldn't get jsonObject from {nameof(firstDocumentJson)} with key {COMPONENTS_TAG}:{SCHEMAS_TAG}");
                }

                if (secondComponentsSection == null) 
                {
                    errorsList.Add($"Couldn't get jsonObject from {nameof(secondDocumentJson)} with key {COMPONENTS_TAG}:{SCHEMAS_TAG}");
                }

                if (firstComponentsSection != null && secondComponentsSection != null)
                {
                    foreach (var component in secondComponentsSection.ToArray())
                    {
                        secondComponentsSection.Remove(component.Key);
                        firstComponentsSection.Add(component);
                    }
                }

                var firstPathsSection = firstDocumentJson[PATH_TAG]?.AsObject()
                    ?? throw new Exception($"Couldn't get jsonObject from {nameof(firstDocumentJson)} with key {PATH_TAG}");
                var secondPathssSection = secondDocumentJson[PATH_TAG]?.AsObject()
                    ?? throw new Exception($"Couldn't get jsonObject from {nameof(secondDocumentJson)} with key {PATH_TAG}");

                foreach (var component in secondPathssSection.ToArray())
                {
                    secondPathssSection.Remove(component.Key);
                    firstPathsSection.Add(component);
                }

                errors = errorsList;
                return firstDocumentJson.ToJsonString();
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        private async Task<string?> GetDocumentationForService(ServiceConfig serviceConfig)
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

                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" exception: {ex}", serviceConfig, ex);
                return null;
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, string message, int statusCode, string contentType = "text/html")
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;
            var bytes = Encoding.UTF8.GetBytes(message);
            await context.Response.Body.WriteAsync(bytes);
        }
    }
}