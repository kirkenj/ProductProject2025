using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Application.Models.User;
using AuthAPI.Models.Requests;
using AuthService.API.AuthAPI.ActionFIlters;
using AuthService.API.AuthAPI.Contracts;
using AuthService.API.AuthAPI.Models.Requests;
using AuthService.Core.Application.Features.User.Commands.ConfirmEmailChangeComand;
using AuthService.Core.Application.Features.User.Commands.SendTokenToUpdateUserEmailRequest;
using AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand;
using AuthService.Core.Application.Features.User.Commands.UpdateUserLoginCommand;
using AuthService.Core.Application.Features.User.Commands.UpdateUserPasswordCommand;
using AuthService.Core.Application.Features.User.Commands.UpdateUserRoleCommand;
using AuthService.Core.Application.Features.User.Queries.GetUserDetailQuery;
using AuthService.Core.Application.Features.User.Queries.GetUserListQuery;
using AuthService.Core.Application.Models.DTOs.User;
using ClaimsPrincipalExtensions;
using Constants;
using CustomResponse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenTracker<Guid> _tokenTracker;

        public UsersController(IMediator mediator, ITokenTracker<Guid> tokenTracker)
        {
            _mediator = mediator;
            _tokenTracker = tokenTracker;
        }

        [HttpGet("list")]
        [GetUserActionFilter]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] UserFilter filter, int? page, int? pageSize)
        {
            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME))
            {
                filter.RoleIds = null;
                filter.Address = null;
            }

            Response<List<UserDto>> result = await _mediator.Send(new GetUserListQuery() { UserFilter = filter, Page = page, PageSize = pageSize });
            return result.GetActionResult();
        }

        [HttpGet("{id}")]
        [HttpGet("Me")]
        [GetUserActionFilter]
        public async Task<ActionResult<UserDto>> Get([AllowNull] Guid? id)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            Response<UserDto> result = await _mediator.Send(new GetUserDetailQuery() { Id = id.Value });
            return result.GetActionResult();
        }

        [HttpPut("{id}")]
        [HttpPut("Me")]
        [Authorize]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> Put([AllowNull] Guid? id, UpdateUserModel updateUserModel)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME) && id != User.GetUserId())
            {
                return Forbid();
            }

            Response<string> result = await _mediator.Send(new UpdateNotSensitiveUserInfoCommand
            {
                Id = id.Value,
                Address = updateUserModel.Address,
                Name = updateUserModel.Name
            });

            return result.GetActionResult();
        }

        [HttpPut("{id}/Email")]
        [HttpPut("Me/Email")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdateEmail(Guid? id, [FromBody][EmailAddress] string newEmail)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME) && id != User.GetUserId())
            {
                return Forbid();
            }

            Response<string> result = await _mediator.Send(new SendTokenToUpdateUserEmailCommand
            {
                Id = id.Value,
                Email = newEmail
            });

            return result.GetActionResult();
        }

        [HttpPost("{id}/Email/Confirm")]
        [HttpPost("Me/Email/Confirm")]
        [Authorize]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> ConfirmEmailUpdate(Guid? id, ConfirmEmailChange confirmEmailChangeAdmin)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME) && id != User.GetUserId())
            {
                return Forbid();
            }

            Response<string> result = await _mediator.Send(new ConfirmEmailChangeComand
            {
                Id = id.Value,
                OtpToNewEmail = confirmEmailChangeAdmin.OtpToNewEmail,
                OtpToOldEmail = confirmEmailChangeAdmin.OtpToOldEmail
            });

            if (result.Success)
            {
                await _tokenTracker.InvalidateUser(id.Value, DateTime.UtcNow);
            }

            return result.GetActionResult();
        }

        [HttpPut("{id}/Login")]
        [HttpPut("Me/Login")]
        [Produces("text/plain")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateLogin(Guid? id, string newLogin)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME) && id != User.GetUserId())
            {
                return Forbid();
            }

            Response<string> result = await _mediator.Send(new UpdateUserLoginCommand
            {
                Id = id.Value,
                NewLogin = newLogin
            });

            if (result.Success)
            {
                await _tokenTracker.InvalidateUser(id.Value, DateTime.UtcNow);
            }

            return result.GetActionResult();
        }

        [HttpPut("{id}/Role")]
        [HttpPut("Me/Role")]
        [Authorize(ApiConstants.ADMIN_AUTH_POLICY_NAME)]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdateRole(Guid id, int roleId)
        {
            Response<string> result = await _mediator.Send(new UpdateUserRoleCommand
            {
                Id = id,
                RoleID = roleId
            });

            if (result.Success)
            {
                await _tokenTracker.InvalidateUser(id, DateTime.UtcNow);
            }

            return result.GetActionResult();
        }

        [HttpPut("Password")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> UpdatePassword(Guid? id, [FromBody] string newPassword)
        {
            id ??= User.GetUserId();
            if (id == null || !id.HasValue)
            {
                return NotFound();
            }

            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME) && id != User.GetUserId())
            {
                return Forbid();
            }

            Response<string> result = await _mediator.Send(new UpdateUserPasswordCommand
            {
                Id = id.Value,
                Password = newPassword
            });

            return result.GetActionResult();
        }
    }
}