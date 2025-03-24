using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand
{
    public class SendTokenToUpdateUserEmailRequest : IRequest<Response<string>>, IEmailUpdateDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
    }
}
