using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Domain.Models;
using HashProvider.Contracts;

namespace AuthService.Core.Application.Services
{
    public class PasswordSetter : IPasswordSetter
    {
        public IHashProvider HashPrvider { get; private set; }

        public PasswordSetter(IHashProvider hashProvider)
        {
            this.HashPrvider = hashProvider;
        }

        public User SetPassword(string password, User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrWhiteSpace(password))
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
