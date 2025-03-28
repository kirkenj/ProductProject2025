using AuthService.Core.Domain.Models;

namespace AuthService.Core.Application.Contracts.Application
{
    public interface IPasswordSetter
    {
        public User SetPassword(string password, User user);
    }
}
