using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.DTOs;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.GetUserDto
{
    public class GetUserDetailHandler : IRequestHandler<GetUserDtoRequest, Response<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserDetailHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(GetUserDtoRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.Id);

            return user == null ?
                Response<UserDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<UserDto>.OkResponse(_mapper.Map<UserDto>(user), "Success");
        }
    }
}
