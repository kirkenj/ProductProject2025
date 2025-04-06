using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComand : IRequest<Response<string>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string OtpToNewEmail { get; set; } = null!;
        public string OtpToOldEmail { get; set; } = null!;
    }
}

