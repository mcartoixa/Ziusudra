using Microsoft.UI.Dispatching;
using Ziusudra.Client;

namespace Ziusudra.Desktop
{

    /// <summary>An <see cref="IUIDispatcher" /> backed by a WinUI <see cref="DispatcherQueue" />.</summary>
    public sealed class UIDispatcher:
        IUIDispatcher
    {

        /// <summary>Create a new instance of the <see cref="UIDispatcher" /> type.</summary>
        /// <param name="dispatcherQueue">The UI thread's dispatcher queue.</param>
        public UIDispatcher(DispatcherQueue dispatcherQueue)
        {
            ArgumentNullException.ThrowIfNull(dispatcherQueue);
            _DispatcherQueue = dispatcherQueue;
        }

        /// <summary>Posts the specified <paramref name="action" /> to run on the UI thread.</summary>
        /// <param name="action">The action to run.</param>
        public void Post(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _DispatcherQueue.TryEnqueue(() => action());
        }

        private readonly DispatcherQueue _DispatcherQueue;
    }
}
