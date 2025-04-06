using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Queries.GetUserDetailQuery
{
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, Response<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.Id);

            return user == null ?
                Response<UserDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<UserDto>.OkResponse(_mapper.Map<UserDto>(user), "Success");
        }
    }
}
