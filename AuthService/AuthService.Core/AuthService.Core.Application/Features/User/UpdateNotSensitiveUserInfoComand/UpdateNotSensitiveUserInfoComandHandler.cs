using AuthService.Core.Application.Contracts.Persistence;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand
{
    public class UpdateNotSensitiveUserInfoComandHandler : IRequestHandler<UpdateNotSensitiveUserInfoComand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateNotSensitiveUserInfoComandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<string>> Handle(UpdateNotSensitiveUserInfoComand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.UpdateUserInfoDto.Id);

            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(request.UpdateUserInfoDto.Id), true);
            }

            _mapper.Map(request.UpdateUserInfoDto, user);

            await _userRepository.UpdateAsync(user);

            return Response<string>.OkResponse("Ok", "Success");
        }
    }
}
