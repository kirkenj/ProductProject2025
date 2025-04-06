using AuthService.Core.Application.Contracts.Persistence;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand
{
    public class UpdateNotSensitiveUserInfoCommandHandler : IRequestHandler<UpdateNotSensitiveUserInfoCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateNotSensitiveUserInfoCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<string>> Handle(UpdateNotSensitiveUserInfoCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(request.Id), true);
            }

            _mapper.Map(request, user);

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Response<string>.OkResponse("Ok", "Success");
        }
    }
}