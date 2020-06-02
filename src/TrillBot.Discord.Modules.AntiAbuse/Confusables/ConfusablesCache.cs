using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Discord.Modules.AntiAbuse.Confusables
{
    internal sealed class ConfusablesCache
    {
        private const string ResourceUrl = "https://www.unicode.org/Public/security/latest/confusables.txt";
        private static readonly TimeSpan CacheMaxAge = TimeSpan.FromDays(1);

        private readonly IDictionary<uint, IEnumerable<uint>> _cache = new Dictionary<uint, IEnumerable<uint>>();
        private DateTime? _cacheUpdated;

        public async Task<IDictionary<uint, IEnumerable<uint>>> GetAsync(CancellationToken cancellationToken = default)
        {
            await EnsureFetchedAsync(cancellationToken);
            return _cache;
        }

        private async Task EnsureFetchedAsync(CancellationToken cancellationToken = default)
        {
            if (_cacheUpdated.HasValue && DateTime.UtcNow - _cacheUpdated.Value < CacheMaxAge) return;
            await FetchAsync(cancellationToken);
        }

        private async Task FetchAsync(CancellationToken cancellationToken = default)
        {
            var newlineSeparators = new[] {"\r\n", "\n", "\r"};
            const char commentSeparator = '#';
            const char entrySeparator = ';';
            const char prototypeSeparator = ' ';

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(ResourceUrl);

            var mappings = response
                .Split(newlineSeparators, StringSplitOptions.None)
                .Select(_ => _.Split(commentSeparator, 2)[0])
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(_ =>
                {
                    var entries = _
                        .Split(entrySeparator)
                        .ToList();
                    var source = uint.Parse(entries[0].Trim(), NumberStyles.HexNumber);
                    var prototypes = entries[1]
                        .Split(prototypeSeparator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(prototype => uint.Parse(prototype.Trim(), NumberStyles.HexNumber));
                    return (source, prototypes);
                });

            _cache.Clear();
            foreach (var (source, prototypes) in mappings) _cache[source] = prototypes;

            _cacheUpdated = DateTime.UtcNow;
        }
    }
}