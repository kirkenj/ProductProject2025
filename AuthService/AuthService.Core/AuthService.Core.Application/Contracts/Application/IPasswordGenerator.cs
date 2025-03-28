namespace AuthService.Core.Application.Contracts.Application
{
    public interface IPasswordGenerator
    {
        public string Generate(int length = 8);
    }
}
