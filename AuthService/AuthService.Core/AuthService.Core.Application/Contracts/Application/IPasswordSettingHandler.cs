using AuthService.Core.Domain.Models;
using HashProvider.Contracts;

namespace AuthService.Core.Application.Contracts.Application
{
    public interface IPasswordSettingHandler
    {
        public IHashProvider HashPrvider { get; }

        public User SetPassword(string password, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            user.PasswordHash = HashPrvider.GetHash(password);
            user.StringEncoding = HashPrvider.Encoding.BodyName;
            user.HashAlgorithm = HashPrvider.HashAlgorithmName;
            return user;
        }
    }
}
