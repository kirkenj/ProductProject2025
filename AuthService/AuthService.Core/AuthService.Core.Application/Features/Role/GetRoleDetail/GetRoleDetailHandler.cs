using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.Role;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleDetail
{
    public class GetRoleDetailHandler : IRequestHandler<GetRoleDtoRequest, Response<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRoleDetailHandler(IRoleRepository userRepository, IMapper mapper)
        {
            _roleRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<RoleDto>> Handle(GetRoleDtoRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetAsync(request.Id);
            return role == null ?
                Response<RoleDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<RoleDto>.OkResponse(_mapper.Map<RoleDto>(role), string.Empty);
        }
    }
}
