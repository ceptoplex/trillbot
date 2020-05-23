using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels
{
    internal sealed class Awaiter<T>
    {
        private readonly ICollection<T> _awaited = new List<T>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public bool CurrentlyWaiting => _awaited.Any();

        public async Task WaitForAsync(Func<Task<T>> awaitedProducer)
        {
            await _semaphore.WaitAsync();
            WaitFor(await awaitedProducer());
            _semaphore.Release();
        }

        public async Task WaitForAsync(Func<IAsyncEnumerable<T>> awaitedProducer)
        {
            await _semaphore.WaitAsync();
            await foreach (var awaited in awaitedProducer())
                WaitFor(awaited);
            _semaphore.Release();
        }

        public async Task StopWaitingForAsync(T awaited)
        {
            await _semaphore.WaitAsync();
            _awaited.Remove(awaited);
            _semaphore.Release();
        }

        private void WaitFor(T awaited)
        {
            _awaited.Add(awaited);
        }
    }
}