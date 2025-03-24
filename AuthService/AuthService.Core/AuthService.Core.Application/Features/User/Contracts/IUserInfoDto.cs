namespace AuthService.Core.Application.Features.User.Interfaces
{
    public interface IUserInfoDto
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
