using AuthService.Core.Application.Features.Role.Queries.GetRoleDtoQuery;
using AuthService.Core.Application.Features.Role.Queries.GetRoleListRequest;
using AuthService.Core.Application.Models.DTOs.Role;
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
            Response<List<RoleDto>> result = await _mediator.Send(new GetRoleListQuery());
            return result.GetActionResult();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            Response<RoleDto> result = await _mediator.Send(new GetRoleDtoQuery() { Id = id });
            return result.GetActionResult();
        }
    }
}
