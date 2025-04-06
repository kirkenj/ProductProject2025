using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Queries.GetUserListQuery
{
    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, Response<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserListQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<UserDto>>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<Domain.Models.User> users = await _userRepository.GetPageContentAsync(request.UserFilter, request.Page, request.PageSize);
            return Response<List<UserDto>>.OkResponse(_mapper.Map<List<UserDto>>(users), "Success");
        }
    }
}
