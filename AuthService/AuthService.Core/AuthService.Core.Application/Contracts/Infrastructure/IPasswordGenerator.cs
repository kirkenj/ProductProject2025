namespace AuthService.Core.Application.Contracts.Infrastructure
{
    public interface IPasswordGenerator
    {
        public string Generate(int length = 8);
    }
}
