namespace Ziusudra.Client
{

    /// <summary>Marshals an action onto the UI thread.</summary>
    /// <remarks>The session layer is thread-agnostic and raises its notifications on whichever thread produced them.
    /// View-models use this seam to project those notifications onto the UI thread. The Desktop layer supplies a
    /// <c>DispatcherQueue</c>-backed implementation; tests supply a synchronous one.</remarks>
    public interface IUIDispatcher
    {

        /// <summary>Posts the specified <paramref name="action" /> to run on the UI thread.</summary>
        /// <param name="action">The action to run.</param>
        void Post(Action action);
    }
}
