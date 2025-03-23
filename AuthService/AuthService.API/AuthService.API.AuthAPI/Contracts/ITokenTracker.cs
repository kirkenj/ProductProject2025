using HashProvider.Contracts;

namespace AuthAPI.Contracts
{
    public interface ITokenTracker<TUserIdType> where TUserIdType : struct
    {
        IHashProvider HashProvider { get; }

        Task InvalidateUser(TUserIdType userId, DateTime time);
        Task<bool> IsValid(string tokenHash);
        Task Track(string token, TUserIdType userId, DateTime tokenRegistrationTime);
    }
}