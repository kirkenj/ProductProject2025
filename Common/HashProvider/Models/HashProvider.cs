using System.Security.Cryptography;
using System.Text;
using HashProvider.Contracts;
using Microsoft.Extensions.Options;

namespace HashProvider.Models
{
    public class HashProvider : IHashProvider
    {
        private string _hashAlgorithmName;
        private HashAlgorithm _hashFunction;

        public HashProvider(IOptions<HashProviderSettings> options) : this(options?.Value ?? throw new ArgumentNullException(nameof(options)))
        {
        }

        public HashProvider(HashProviderSettings options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _hashFunction = GetHashAlgorithm(options.HashAlgorithmName) ?? throw new ArgumentException(nameof(options.HashAlgorithmName));
            _hashAlgorithmName = options.HashAlgorithmName;
            Encoding = Encoding.GetEncoding(options.EncodingName);
        }

        public string HashAlgorithmName
        {
            get => _hashAlgorithmName;
            set
            {
                _hashFunction = GetHashAlgorithm(value) ?? throw new ArgumentException($"Hash algorithm with name {value} not found");
                _hashAlgorithmName = value;
            }
        }

        public HashAlgorithm HashFunction { get => _hashFunction; }

        public Encoding Encoding { get; set; }

#pragma warning disable SYSLIB0045 // Type or member is obsolete
        private static HashAlgorithm? GetHashAlgorithm(string name) => HashAlgorithm.Create(name);
#pragma warning restore SYSLIB0045 // Type or member is obsolete
    }
}
