using System.Threading.Tasks;

namespace TrillBot.Common.Events
{
    public delegate Task AsyncEventHandler<TCancelableEventArgs>(object sender, TCancelableEventArgs e)
        where TCancelableEventArgs : CancelableEventArgs;
}