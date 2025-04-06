using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.SendTokenToUpdateUserEmailRequest
{
    public class SendTokenToUpdateUserEmailCommand : IRequest<Response<string>>, IIdObject<Guid>, IEmailDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
    }
}
