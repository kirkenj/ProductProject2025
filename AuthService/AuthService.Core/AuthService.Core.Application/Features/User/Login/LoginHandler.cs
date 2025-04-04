using System.Text;
using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using HashProvider.Contracts;
using MediatR;

namespace AuthService.Core.Application.Features.User.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, Response<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashProvider _hashProvider;
        private readonly IMapper _mapper;

        public LoginHandler(IUserRepository userRepository,
            IRoleRepository roleRepository,
            IHashProvider hashProvider,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _hashProvider = hashProvider;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var userToHandle = await _userRepository.GetAsync(new UserFilter { AccurateEmail = request.Email });
            if (userToHandle == null)
            {
                return Response<UserDto>.BadRequestResponse("Wrong password or email");
            }

            _hashProvider.HashAlgorithmName = userToHandle.HashAlgorithm;
            _hashProvider.Encoding = Encoding.GetEncoding(userToHandle.StringEncoding);

            string loginPasswordHash = _hashProvider.GetHash(request.Password);

            if (loginPasswordHash != userToHandle.PasswordHash)
            {
                return Response<UserDto>.BadRequestResponse("Wrong password or email");
            }

            UserDto userDto = _mapper.Map<UserDto>(userToHandle);

            return Response<UserDto>.OkResponse(userDto, "Success");
        }
    }
}
