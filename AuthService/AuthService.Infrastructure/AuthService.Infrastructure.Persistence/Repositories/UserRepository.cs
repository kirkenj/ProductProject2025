using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Domain.Models;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace AuthService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericFiltrableRepository<User, Guid, UserFilter>, IUserRepository
    {
        public UserRepository(AuthDbContext dbContext) : base(dbContext, GetFilteredSet)
        {
        }

        private static IQueryable<User> GetFilteredSet(IQueryable<User> set, UserFilter filter)
        {
            if (filter == null)
            {
                return set;
            }

            if (filter.Ids != null && filter.Ids.Any())
            {
                set = set.Where(obj => filter.Ids.Contains(obj.Id));
            }

            if (filter.RoleIds != null && filter.RoleIds.Any())
            {
                set = set.Where(obj => filter.RoleIds.Contains(obj.RoleID));
            }

            if (!string.IsNullOrEmpty(filter.Address))
            {
                set = set.Where(obj => obj.Address.Contains(filter.Address));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                set = set.Where(obj => obj.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.AccurateEmail))
            {
                set = set.Where(obj => obj.Email != null && obj.Email == filter.AccurateEmail);
            }

            if (!string.IsNullOrEmpty(filter.EmailPart))
            {
                set = set.Where(obj => obj.Email != null && obj.Email.Contains(filter.EmailPart));
            }

            if (!string.IsNullOrEmpty(filter.LoginPart))
            {
                set = set.Where(obj => obj.Login != null && obj.Login.Contains(filter.LoginPart));
            }

            if (!string.IsNullOrEmpty(filter.AccurateLogin))
            {
                set = set.Where(obj => obj.Login != null && obj.Login == filter.AccurateLogin);
            }

            return set;
        }
    }
}
