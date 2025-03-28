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
        private static readonly List<string[]> SECTION_KEYS_TO_COPY =
        [
            ["components", "schemas"],
            ["paths"]
        ];

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

            var resultDocument = CombineDocuments(openApiDocuments);
            if (resultDocument == null)
            {
                var message = "Couldn't aggregate openApi docs";
                _logger.LogError(message);
                await WriteResponseAsync(context, message, 500);
                return;
            }

            var openApiDoc = new OpenApiStringReader().Read(resultDocument, out var diagnostic);
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

        private string? CombineDocuments(IEnumerable<(string Url, string? openApiDocument)> uriAndJsonsToHandle)
        {
            if (uriAndJsonsToHandle == null)
            {
                return null;
            }

            List<(string uri, JsonObject openApiDocumentJson)> filteredDocuments = [];
            foreach (var documentString in uriAndJsonsToHandle)
            {
                if (string.IsNullOrEmpty(documentString.openApiDocument))
                {
                    _logger.LogWarning("Couldn't recieve openApi document from {uri}", documentString.Url);
                    continue;
                }

                var jsonObj = JsonObject.Parse(documentString.openApiDocument)?.AsObject();
                if (jsonObj == null)
                {
                    _logger.LogWarning("Couldn't parse jsonObject from openApi documentation {uri}", documentString.Url);
                    continue;
                }

                filteredDocuments.Add((documentString.Url, jsonObj));
            }

            if (filteredDocuments.Count == 0)
            {
                return null;
            }

            (var resultJson, var resultJsonSectionsDict) = InitialOpenApiJsonObjectFactory();

            foreach (var uriAndJson in filteredDocuments)
            {
                foreach (var key in SECTION_KEYS_TO_COPY)
                {
                    var targetSection = resultJsonSectionsDict[key];
                    var sourceSection = GetSectionWithKeys(uriAndJson.openApiDocumentJson, key);

                    if (sourceSection == null)
                    {
                        _logger.LogWarning("Couldn't get section from source json with key \'{key}\'; Uri:{uri}", string.Join(':', key), uriAndJson.uri);
                        continue;
                    }

                    foreach (var component in sourceSection.ToArray())
                    {
                        sourceSection.Remove(component.Key);
                        targetSection.Add(component);
                    }
                }
            }

            return resultJson.ToJsonString();
        }

        private static (JsonObject, Dictionary<string[], JsonObject>) InitialOpenApiJsonObjectFactory()
        {
            var resultJson = JsonObject.Parse("""
                {
                    "openapi": "3.0.4",
                    "info": {
                        "title": "SomeTitle",
                        "version": "1.0"
                    },
                    "paths": {},
                    "components": {
                        "schemas": {}
                    }
                }
                """)?.AsObject()!;

            var resultJsonSectionsDict = SECTION_KEYS_TO_COPY.Select(s =>
            {
                var targetSection = GetSectionWithKeys(resultJson, s) ?? throw new InvalidOperationException();
                return new KeyValuePair<string[], JsonObject>(s, targetSection);
            }).ToDictionary();

            return (resultJson, resultJsonSectionsDict);
        }

        private static JsonObject? GetSectionWithKeys(JsonObject source, string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            JsonNode? result = source[keys[0]];

            for (int i = 1; i < keys.Length; i++)
            {
                if (result == null)
                {
                    return null;
                }

                result = result[keys[i]];
            }

            return result?.AsObject();
        }

        private async Task<(string Url, string? openApiDocument)> GetDocumentationForService(ServiceConfig serviceConfig)
        {
            var uri = $"{serviceConfig.DownstreamScheme}://{serviceConfig.Host}:{serviceConfig.Port}/{serviceConfig.SwaggerUrl}";
            try
            {
                var result = await _httpClient.GetAsync(uri);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" with uri \"{serviceSwaggerUrl}\" ({code})", serviceConfig, uri, result.StatusCode);
                    return (uri, null);
                }

                return (uri, await result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't get OpenApi documentation for service \"{serviceName}\" exception: {ex}", serviceConfig, ex.Message);
                return (uri, null);
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