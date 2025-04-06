using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;


namespace AuthService.Core.Application.Features.User.Commands.ForgotPasswordCommand
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordComand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducer<ForgotPassword> _forgotPasswordProducer;
        private readonly IPasswordSetter _passwordSetter;
        private readonly IPasswordGenerator _passwordGenerator;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IKafkaProducer<ForgotPassword> forgotPasswordProducer,
            IPasswordSetter passwordSetter,
            IPasswordGenerator passwordGenerator)
        {
            _userRepository = userRepository;
            _forgotPasswordProducer = forgotPasswordProducer;
            _passwordSetter = passwordSetter;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<Response<string>> Handle(ForgotPasswordComand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(new UserFilter { AccurateEmail = request.Email }, cancellationToken);

            var response = Response<string>.OkResponse("New password was sent on your email", "Success");
            if (user == null)
            {
                return response;
            }

            string newPassword = _passwordGenerator.Generate();

            _passwordSetter.SetPassword(newPassword, user);
            await _userRepository.UpdateAsync(user);

            await _forgotPasswordProducer.ProduceAsync(new ForgotPassword { UserId = user.Id, NewPassword = newPassword }, cancellationToken);

            return response;
        }
    }
}