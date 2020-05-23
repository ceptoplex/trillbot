using System;
using System.Threading;

namespace TrillBot.Common.Events
{
    public class CancelableEventArgs : EventArgs
    {
        public new static readonly CancelableEventArgs Empty = new CancelableEventArgs();

        public CancelableEventArgs(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; set; }
    }
}