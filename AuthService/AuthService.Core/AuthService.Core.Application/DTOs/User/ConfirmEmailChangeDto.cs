using Repository.Contracts;

namespace AuthService.Core.Application.DTOs.User
{
    public class ConfirmEmailChangeDto : IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = null!;
    }
}
