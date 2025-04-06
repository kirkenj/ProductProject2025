using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.Role;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.Queries.GetRoleDtoQuery
{
    public class GetRoleDtoQueryHandler : IRequestHandler<GetRoleDtoQuery, Response<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRoleDtoQueryHandler(IRoleRepository userRepository, IMapper mapper)
        {
            _roleRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<RoleDto>> Handle(GetRoleDtoQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetAsync(request.Id);
            return role == null ?
                Response<RoleDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<RoleDto>.OkResponse(_mapper.Map<RoleDto>(role), string.Empty);
        }
    }
}
