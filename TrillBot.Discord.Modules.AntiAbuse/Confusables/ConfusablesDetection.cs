using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fastenshtein;

namespace TrillBot.Discord.Modules.AntiAbuse.Confusables
{
    // See: https://www.unicode.org/reports/tr39/#Confusable_Detection
    internal class ConfusablesDetection
    {
        private readonly ConfusablesCache _confusablesCache;

        public ConfusablesDetection(ConfusablesCache confusablesCache)
        {
            _confusablesCache = confusablesCache;
        }

        public async Task<bool> TestConfusabilityAsync(string actual, string tested)
        {
            var minRequiredDistance = actual.Length / 3;

            var preparedActual = await PrepareAsync(actual);
            var preparedTested = await PrepareAsync(tested);
            return Levenshtein.Distance(preparedActual, preparedTested) < minRequiredDistance;

            async Task<string> PrepareAsync(string input)
            {
                var output = input;
                output = await GetSkeletonAsync(output);
                output = output.ToLowerInvariant();
                return output;
            }
        }

        private async Task<string> GetSkeletonAsync(string input)
        {
            var output = input;
            output = output.Normalize(NormalizationForm.FormD);
            output = await ReplaceConfusablesAsync(output);
            output = output.Normalize(NormalizationForm.FormD);
            return output;
        }

        private async Task<string> ReplaceConfusablesAsync(string input)
        {
            var confusables = await _confusablesCache.GetAsync();

            var encoding = Encoding.UTF32;
            const int characterByteCount = 4;

            var inputBytes = encoding.GetBytes(input);
            var inputCharacters = Enumerable
                .Range(0, inputBytes.Length / characterByteCount)
                .Select(_ => BitConverter.ToUInt32(inputBytes, _ * characterByteCount));
            var outputCharacters = inputCharacters
                .SelectMany(_ => confusables.TryGetValue(_, out var replacement)
                    ? replacement
                    : Enumerable.Repeat(_, 1));
            var outputBytes = outputCharacters
                .SelectMany(BitConverter.GetBytes);

            return encoding.GetString(outputBytes.ToArray());
        }
    }
}