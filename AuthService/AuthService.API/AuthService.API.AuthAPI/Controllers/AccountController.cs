using AuthAPI.Models.Requests;
using AuthService.API.AuthAPI.Contracts;
using AuthService.API.AuthAPI.Models.Requests;
using AuthService.Core.Application.Features.User.ConfirmEmailChangeComand;
using AuthService.Core.Application.Features.User.DTOs;
using AuthService.Core.Application.Features.User.GetUserDto;
using AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand;
using AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand;
using AuthService.Core.Application.Features.User.UpdateUserLoginComand;
using AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler;
using CustomResponse;
using Extensions.ClaimsPrincipalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenTracker<Guid> _tokenTracker;

        public AccountController(IMediator mediator, ITokenTracker<Guid> tokenTracker)
        {
            _tokenTracker = tokenTracker;
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            Response<UserDto> result = await _mediator.Send(new GetUserDtoRequest()
            {
                Id = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id")
            });
            return result.GetActionResult();
        }

        [HttpPut]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> Put(UpdateUserModel updateUserModel)
        {
            Response<string> result = await _mediator.Send(new UpdateNotSensitiveUserInfoComand
            {
                Id = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id"),
                Address = updateUserModel.Address,
                Name = updateUserModel.Name
            });

            return result.GetActionResult();
        }

        [HttpPut("Password")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdatePassword([FromBody] string request)
        {
            Response<string> result = await _mediator.Send(new UpdateUserPasswordComand
            {
                Id = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id"),
                Password = request
            });

            return result.GetActionResult();
        }

        [HttpPut("Login")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdateLogin(string newLogin)
        {
            var userId = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id");

            Response<string> result = await _mediator.Send(new UpdateUserLoginComand
            {
                Id = userId,
                NewLogin = newLogin
            });

            if (result.Success)
            {
                result.Result += "Relogin needed.";
                await _tokenTracker.InvalidateUser(userId, DateTime.UtcNow);
            }

            return result.GetActionResult();
        }

        [HttpPut("Email")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdateEmail([FromBody] string newEmail)
        {
            Response<string> result = await _mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                Email = newEmail,
                Id = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id")
            });

            return result.GetActionResult();
        }

        [HttpPost("Email/Confirm")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> ConfirmEmailUpdate([FromBody] ConfirmEmailChange confirmEmailChange)
        {
            Guid userId = User.GetUserId() ?? throw new ApplicationException("Couldn't get user's id");
            Response<string> result = await _mediator.Send(new ConfirmEmailChangeComand
            {
                Id = userId,
                OtpToNewEmail = confirmEmailChange.OtpToNewEmail,
                OtpToOldEmail = confirmEmailChange.OtpToOldEmail,
            });

            if (result.Success)
            {
                result.Result += "Relogin needed.";
                await _tokenTracker.InvalidateUser(userId, DateTime.UtcNow);
            }

            return result.GetActionResult();
        }
    }
}
