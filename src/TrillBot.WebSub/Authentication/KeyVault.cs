using System;
using System.Security.Cryptography;
using System.Text;

namespace TrillBot.WebSub.Authentication
{
    internal sealed class KeyVault : IKeyVault
    {
        public const int DefaultKeyByteCount = 32;

        private readonly byte[] _keyBytes;

        public KeyVault(int keyByteCount = DefaultKeyByteCount)
        {
            _keyBytes = GenerateKey(keyByteCount);
        }

        public byte[] GetKeyAsBytes()
        {
            return Encoding.ASCII.GetBytes(GetKey());
        }

        public string GetKey()
        {
            return Convert.ToBase64String(_keyBytes);
        }

        private static byte[] GenerateKey(int byteCount)
        {
            var bytes = new byte[byteCount];

            using var crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(bytes);

            return bytes;
        }
    }
}