using System.ComponentModel.DataAnnotations;
using AuthAPI.Contracts;
using AuthAPI.Models.Requests;
using AuthService.Core.Application.DTOs.User;
using AuthService.Core.Application.Features.User.CreateUserComand;
using AuthService.Core.Application.Features.User.ForgotPasswordComand;
using AuthService.Core.Application.Features.User.GetUserDto;
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

        [Produces("text/plain")]
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromBody] CreateUserDto createUserDto)
        {
            Response<Guid> result = await _mediator.Send(new CreateUserCommand() { CreateUserDto = createUserDto });
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return result.GetActionResult();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResultModel>> Login(LoginDto loginDto)
        {
            Response<UserDto> response = await _mediator.Send(new LoginRequest { LoginDto = loginDto });

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
                ForgotPasswordDto = new ForgotPasswordDto
                {
                    Email = email
                }
            });
            return result.GetActionResult();
        }
    }
}
