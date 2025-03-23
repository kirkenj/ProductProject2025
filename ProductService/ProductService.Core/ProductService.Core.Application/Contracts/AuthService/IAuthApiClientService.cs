using ProductService.Core.Application.Models.UserClient;

namespace ProductService.Core.Application.Contracts.AuthService
{
    public interface IAuthApiClientService
    {
        public Task<ClientResponse<AuthClientUser?>> GetUser(Guid userId);
        public Task<ClientResponse<ICollection<AuthClientUser>>?> ListAsync(IEnumerable<Guid>? ids = null, string? accurateLogin = null, string? loginPart = null, string? accurateEmail = null, string? emailPart = null, string? address = null, string? name = null, IEnumerable<int>? roleIds = null, int? page = null, int? pageSize = null);
    }
}