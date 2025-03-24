using CustomResponse;

namespace CentralizedJwtAuthentication
{
    public interface IJwtValidatingService
    {
        public Task<Response<bool>> IsValid(string token);
    }
}
