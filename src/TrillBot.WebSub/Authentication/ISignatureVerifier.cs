using System.Collections.Generic;

namespace TrillBot.WebSub.Authentication
{
    internal interface ISignatureVerifier
    {
        bool VerifySignature(IEnumerable<byte> signature, IEnumerable<byte> message);
    }
}