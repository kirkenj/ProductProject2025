using CustomResponse;

namespace Clients.Adapters.ProductClient.Contracts
{
    public interface IProductClientService
    {
        public Task<Response<ProductDto?>> GetProduct(Guid userId, CancellationToken cancellationToken = default);
    }
}
