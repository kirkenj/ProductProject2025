namespace AuthService.API.AuthAPI.Contracts
{
    public interface ITokenTracker<TUserIdType> where TUserIdType : struct
    {
        Task InvalidateUser(TUserIdType userId, DateTime time);
        Task<bool> IsValid(string tokenHash);
        Task Track(string token, TUserIdType userId, DateTime tokenRegistrationTime);
    }
}