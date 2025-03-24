using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.Role.DTOs;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleDetail
{
    public class GetRoleDetailHandler : IRequestHandler<GetRoleDtoRequest, Response<RoleDto>>
    {
        private readonly IRoleRepository _userRepository;
        private readonly IMapper _mapper;

        public GetRoleDetailHandler(IRoleRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<RoleDto>> Handle(GetRoleDtoRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(request.Id);
            return user == null ?
                Response<RoleDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<RoleDto>.OkResponse(_mapper.Map<RoleDto>(user), string.Empty);
        }
    }
}
