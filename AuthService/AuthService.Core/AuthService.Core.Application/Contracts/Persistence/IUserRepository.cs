using Application.Models.User;
using AuthService.Core.Domain.Models;
using Repository.Contracts;

namespace AuthService.Core.Application.Contracts.Persistence
{
    public interface IUserRepository : IGenericFiltrableRepository<User, Guid, UserFilter>
    {
    }
}
