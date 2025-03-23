using AutoMapper;
using Clients.AuthApi;

namespace ServiceProduct.Tests.Common
{
    public class TestAuthClient : IAuthApiClient
    {
        private readonly IMapper _mapper;
        public readonly List<UserDto> _users = null!;
        private readonly RoleDto roleDto = new()
        {
            Id = 1,
            Name = "Regular"
        };

        public TestAuthClient(IMapper mapper)
        {
            _mapper = mapper;
            _users = new()
            {
                new UserDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test user 1",
                    Address = "Moskow",
                    Email = "meow@mour",
                    Login = "Helldriver",
                    Role = roleDto
                },
                new UserDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test user 2",
                    Address = "Minsk",
                    Email = "qqack@mour",
                    Login = "crazy228",
                    Role = roleDto
                },
                new UserDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test user 2",
                    Address = "Minsk",
                    Email = null,
                    Login = "crazy228",
                    Role = roleDto
                }
            };
        }

        public Task<UserDto> UsersGETAsync(Guid id)
        {
            var result = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(result);
        }

        public Task<UserDto> UsersGETAsync(Guid id, CancellationToken cancellationToken) => UsersGETAsync(id);

        public Task<ICollection<UserDto>> ListAsync(IEnumerable<Guid> ids, string accurateLogin, string loginPart, string email, string address, IEnumerable<int> roleIds, int? page, int? pageSize)
        {
            var q = _mapper.Map<ICollection<UserDto>>(_users);

            var a = GetPageContent(GetFilteredSet(q.AsQueryable(), ids, accurateLogin, loginPart, email, address), page, pageSize);

            var res = _mapper.Map<ICollection<UserDto>>(a) ?? throw new ArgumentNullException();

            return Task.FromResult(res);
        }

        public Task<ICollection<UserDto>> ListAsync(IEnumerable<Guid> ids, string accurateLogin, string loginPart, string email, string address, IEnumerable<int> roleIds, int? page, int? pageSize, CancellationToken cancellationToken)
            => ListAsync(ids, accurateLogin, loginPart, email, address, roleIds, page, pageSize);

        private static IQueryable<UserDto> GetFilteredSet(IQueryable<UserDto> set, IEnumerable<Guid> ids, string accurateLogin, string loginPart, string email, string address)
        {
            if (ids != null && ids.Any())
            {
                set = set.Where(obj => ids.Contains(obj.Id));
            }

            if (!string.IsNullOrEmpty(email))
            {
                set = set.Where(obj => obj.Email != null && obj.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(loginPart))
            {
                set = set.Where(obj => obj.Login != null && obj.Login.Contains(loginPart));
            }

            if (!string.IsNullOrEmpty(accurateLogin))
            {
                set = set.Where(obj => obj.Login != null && obj.Login == accurateLogin);
            }

            return set;
        }

        protected static IQueryable<UserDto> GetPageContent(IQueryable<UserDto> query, int? page = default, int? pageSize = default)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var pageVal = page.Value <= 0 ? 1 : page.Value;
                var pageSizeVal = pageSize.Value <= 0 ? 1 : pageSize.Value;
                query = query.Skip((pageVal - 1) * pageSizeVal).Take(pageSizeVal);
            }

            return query;
        }


        #region NotImplementedException
        public Task<UserDto> AccountGETAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> AccountGETAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> AccountPUTAsync(UpdateUserModel body)
        {
            throw new NotImplementedException();
        }

        public Task<string> AccountPUTAsync(UpdateUserModel body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPOST2Async(ConfirmEmailChangeDto body)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPOST2Async(ConfirmEmailChangeDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPOSTAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPOSTAsync(string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPUT2Async(UpdateUserEmailDto body)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPUTAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPUTAsync(string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> ForgotPasswordAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task<string> ForgotPasswordAsync(string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HashProviderSettings> GetHashDefaultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HashProviderSettings> GetHashDefaultsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task InvalidateUsersTokenAsync(Guid? userId)
        {
            throw new NotImplementedException();
        }

        public Task InvalidateUsersTokenAsync(Guid? userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTokenValidAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTokenValidAsync(string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResultModel> LoginAsync(LoginDto body)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResultModel> LoginAsync(LoginDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> PasswordAsync(string body)
        {
            throw new NotImplementedException();
        }

        public Task<string> PasswordAsync(string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> RegisterAsync(CreateUserDto body)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> RegisterAsync(CreateUserDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> RoleAsync(UpdateUserRoleDTO body)
        {
            throw new NotImplementedException();
        }

        public Task<string> RoleAsync(UpdateUserRoleDTO body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<RoleDto>> RolesAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<RoleDto>> RolesAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto> RolesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto> RolesAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<StringStringKeyValuePair>> TokenClaimsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<StringStringKeyValuePair>> TokenClaimsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> UsersPUTAsync(UpdateUserInfoDto body)
        {
            throw new NotImplementedException();
        }

        public Task<string> UsersPUTAsync(UpdateUserInfoDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> UserTag2Async(UpdateUserLoginDto body)
        {
            throw new NotImplementedException();
        }

        public Task<string> UserTag2Async(UpdateUserLoginDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> UserTagAsync(string newLogin)
        {
            throw new NotImplementedException();
        }

        public Task<string> UserTagAsync(string newLogin, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IAuthApiClient.RegisterAsync(CreateUserDto body)
        {
            throw new NotImplementedException();
        }

        Task<string> IAuthApiClient.RegisterAsync(CreateUserDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserDto>> ListAsync(IEnumerable<Guid> ids, string accurateLogin, string loginPart, string accurateEmail, string emailPart, string address, string name, IEnumerable<int> roleIds, int? page, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserDto>> ListAsync(IEnumerable<Guid> ids, string accurateLogin, string loginPart, string accurateEmail, string emailPart, string address, string name, IEnumerable<int> roleIds, int? page, int? pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> EmailPUT2Async(UpdateUserEmailDto body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
