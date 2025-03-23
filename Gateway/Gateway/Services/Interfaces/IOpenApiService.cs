namespace Gateway.Services.Interfaces
{
    public interface IOpenApiService
    {
        public Task<string> GetDocumentationForService(string serviceName);
    }
}
