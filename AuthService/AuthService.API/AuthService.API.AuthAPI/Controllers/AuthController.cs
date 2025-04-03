using System.ComponentModel.DataAnnotations;
using AuthAPI.Models.Requests;
using AuthService.API.AuthAPI.Contracts;
using AuthService.Core.Application.Features.User.DTOs;
using AuthService.Core.Application.Features.User.ForgotPasswordComand;
using AuthService.Core.Application.Features.User.Login;
using CustomResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenTracker<Guid> _tokenTracker;
        private readonly IJwtProviderService _jwtProviderService;

        public AuthController(IMediator mediator, ITokenTracker<Guid> tokenTracker, IJwtProviderService jwtProviderService)
        {
            _mediator = mediator;
            _tokenTracker = tokenTracker;
            _jwtProviderService = jwtProviderService;
        }



        [HttpPost("Login")]
        public async Task<ActionResult<LoginResultModel>> Login(LoginRequest loginDto)
        {
            Response<UserDto> response = await _mediator.Send(loginDto);

            if (!response.Success)
            {
                return response.GetActionResult();
            }

            var user = response.Result ?? throw new ApplicationException($"{nameof(response)}' {nameof(response.Result)} is null with success status");

            string token = _jwtProviderService.GetToken(user) ?? throw new ApplicationException("Couldn't get token from jwt service");

            await _tokenTracker.Track(
                token ?? throw new ApplicationException(),
                response.Result.Id,
                DateTime.UtcNow);

            return Ok(new LoginResultModel { Token = token, UserId = user.Id });
        }

        [HttpPost("ForgotPassword")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> ForgotPassword([FromBody][EmailAddress] string email)
        {
            Response<string> result = await _mediator.Send(new ForgotPasswordComand
            {
                Email = email
            });
            return result.GetActionResult();
        }
    }
}
