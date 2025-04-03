using AuthService.Core.Application.Features.User.RegisterConfirmCommand;
using AuthService.Core.Application.Features.User.RegisterUserComand;
using CustomResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Produces("text/plain")]
        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] RegisterUserCommand createUserDto)
        {
            Response<string> result = await _mediator.Send(createUserDto);
            return result.GetActionResult();
        }

        [Produces("text/plain")]
        [HttpPost("Confirm")]
        public async Task<ActionResult<string>> ConfirmRegistration([FromBody] RegisterConfirmCommand createUserDto)
        {
            Response<string> result = await _mediator.Send(createUserDto);
            return result.GetActionResult();
        }
    }
}
