using AuthService.Core.Application.Contracts.Infrastructure;

namespace AuthService.Infrastructure.Infrastructure.PasswordGenerator
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public string Generate(int length)
        {
            Random random = new();

            var result = string.Join("",                                // создаем строку
                Enumerable.Range(0, length)                             // из последовательности длины length
                .Select(i =>
                    i % 2 == 0 ?                                // на четных местах
                (char)('A' + random.Next(26)) + "" :    // генерируем букву
                random.Next(1, 10) + "")              // на нечетных цифру
                );
            return result;
        }
    }
}
