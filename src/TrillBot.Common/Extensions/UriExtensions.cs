using System;

namespace TrillBot.Common.Extensions
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, string relativePath = default, string query = default)
        {
            const char pathSeparator = '/';

            relativePath ??= string.Empty;

            if (query == default)
                query = string.Empty;
            else
                query = query.StartsWith("?")
                    ? query
                    : $"?{query}";

            return new Uri(
                !uri.AbsoluteUri.EndsWith(pathSeparator) && !string.IsNullOrEmpty(relativePath)
                    ? new Uri(uri.AbsoluteUri + pathSeparator)
                    : uri,
                $"{relativePath!.TrimStart(pathSeparator)}{query}");
        }
    }
}