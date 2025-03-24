using AuthService.Core.Application.Features.Role.DTOs;
using AuthService.Core.Application.Features.Role.GetRoleDetail;
using AuthService.Core.Application.Features.Role.GetRoleListRequest;
using CustomResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.AuthAPI.Controllers
{
    [Route("AuthApi/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRolesList()
        {
            Response<List<RoleDto>> result = await _mediator.Send(new GetRoleListRequest());
            return result.GetActionResult();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            Response<RoleDto> result = await _mediator.Send(new GetRoleDtoRequest() { Id = id });
            return result.GetActionResult();
        }
    }
}
