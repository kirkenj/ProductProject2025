﻿using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Domain.Models;
using Cache.Contracts;
using Microsoft.Extensions.Logging;
using Repository.Models;

namespace AuthService.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : GenericCachingRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(AuthDbContext dbContext, ICustomMemoryCache customMemoryCache, ILogger<GenericCachingRepository<Role, int>> logger) : base(dbContext, customMemoryCache, logger)
        {
            СacheTimeoutMiliseconds = int.MaxValue;
        }
    }
}
