using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComand : IRequest<Response<string>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = null!;
    }
}

