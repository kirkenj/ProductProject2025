using AuthService.Core.Domain.Models;
using Repository.Contracts;

namespace AuthService.Core.Application.Contracts.Persistence
{
    public interface IRoleRepository : IGenericRepository<Role, int>
    {
    }
}
