using System.Security.Cryptography;
using System.Text;

namespace HashProvider.Contracts
{
    public interface IHashProvider
    {
        public string HashAlgorithmName { get; set; }
        HashAlgorithm HashFunction { get; }
        Encoding Encoding { get; set; }

        public string GetHash(string password);
    }
}
