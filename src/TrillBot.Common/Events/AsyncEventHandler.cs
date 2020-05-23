using System.Threading.Tasks;

namespace TrillBot.Common.Events
{
    public delegate Task AsyncEventHandler(object sender, CancelableEventArgs e);
}