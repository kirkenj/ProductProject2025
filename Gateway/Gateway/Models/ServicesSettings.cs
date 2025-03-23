namespace Gateway.Models
{
    public class ServicesSettings
    {
        public string OpenApiPathPrefixSegment { get; set; } = string.Empty;
        public string GatewayOpenApiDocumentName { get; set; } = string.Empty;
        public ICollection<ServiceConfig> ServiceConfigs { get; set; } = null!;
    }
    
    public class ServiceConfig
    {
        public string DownstreamScheme { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string SwaggerUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
