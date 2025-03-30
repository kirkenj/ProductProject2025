using AuthService.API.AuthAPI.Contracts;
using Extensions.ClaimsPrincipalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ITokenTracker<Guid> _tokenTracker;

        public TokensController(ITokenTracker<Guid> tokenTracker)
        {
            _tokenTracker = tokenTracker;
        }

        [HttpPost("IsValid")]
        public async Task<ActionResult<bool>> IsTokenValid([FromBody] string token)
        {
            bool result = await _tokenTracker.IsValid(token);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("TerminateSessions")]
        public async Task<ActionResult> InvalidateToken(Guid userId)
        {
            if (!User.IsInRole(Constants.ApiConstants.ADMIN_AUTH_ROLE_NAME) && User.GetUserId() != userId)
            {
                return Forbid();
            }

            await _tokenTracker.InvalidateUser(userId, DateTime.UtcNow);
            return Ok();
        }
    }
}
