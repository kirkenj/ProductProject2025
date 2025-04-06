using CustomResponse;

namespace Clients.Adapters.AuthClient.Contracts
{
    public interface IAuthApiClientService
    {
        public Task<Response<AuthClientUser?>> GetUser(Guid userId, CancellationToken cancellationToken = default);
        public Task<Response<ICollection<AuthClientUser>>?> ListAsync(IEnumerable<Guid>? ids = null, string? accurateLogin = null, string? loginPart = null, string? accurateEmail = null, string? emailPart = null, string? address = null, string? name = null, IEnumerable<int>? roleIds = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default);
    }
}