using System;
using System.Collections.Generic;
using System.Linq;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions
{
    internal static class ListExtensions
    {
        public static bool TryRemoveFirst<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var element = list.FirstOrDefault(predicate);
            if (element == null) return false;
            list.Remove(element);
            return true;
        }
    }
}