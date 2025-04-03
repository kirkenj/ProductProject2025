using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Domain.Models;
using Cache.Contracts;
using Microsoft.Extensions.Logging;
using Repository.Caching;
using Repository.Models.Relational;

namespace AuthService.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : GenericCachingRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(AuthDbContext dbContext, ICustomMemoryCache customMemoryCache, ILogger<RoleRepository> logger) 
            : base(new GenericRepository<Role, int>(dbContext), customMemoryCache, logger)
        {
            СacheTimeoutMiliseconds = int.MaxValue;
        }
    }
}
