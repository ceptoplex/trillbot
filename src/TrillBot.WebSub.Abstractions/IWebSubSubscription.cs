using TrillBot.Common.Events;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    public interface IWebSubSubscription<TContent>
    {
        event AsyncEventHandler<WebSubContentAvailableEventArgs<TContent>> ContentAvailable;
    }
}