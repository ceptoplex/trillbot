using System;
using System.Collections.Generic;
using System.Linq;

namespace TrillBot.Common
{
    public static class ConversionUtil
    {
        public static IEnumerable<byte> FromHexString(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }
    }
}