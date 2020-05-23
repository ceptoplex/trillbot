using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TrillBot.WebSub.Authentication
{
    internal sealed class SignatureVerifier : ISignatureVerifier
    {
        private readonly IKeyVault _keyVault;

        public SignatureVerifier(IKeyVault keyVault)
        {
            _keyVault = keyVault;
        }

        public bool VerifySignature(IEnumerable<byte> signature, IEnumerable<byte> message)
        {
            return new HMACSHA256(_keyVault.GetKeyAsBytes())
                .ComputeHash(message.ToArray())
                .SequenceEqual(signature);
        }
    }
}