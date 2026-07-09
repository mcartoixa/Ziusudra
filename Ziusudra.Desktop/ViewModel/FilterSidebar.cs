using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Ziusudra.Client;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Projects the daemon's filter tree into the sidebar and applies the selected filter to the torrent list.</summary>
    /// <remarks>The tree is refetched after every monitor poll so counts stay live; updates are merged into the existing
    /// nodes in place so the tree's selection and expansion survive. Like the other view-models, the session raises its
    /// notifications on background threads, so every model mutation is posted through <see cref="IUIDispatcher" />.</remarks>
    public sealed partial class FilterSidebar:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="FilterSidebar" /> type.</summary>
        /// <param name="session">The session whose filter tree is projected.</param>
        /// <param name="dispatcher">The UI dispatcher.</param>
        public FilterSidebar(DelugeSession session, IUIDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(dispatcher);

            _Session = session;
            _Dispatcher = dispatcher;
            _Session.StateChanged += OnSessionStateChanged;
        }

        /// <summary>Gets the category header nodes, each holding its selectable filter values.</summary>
        public ObservableCollection<FilterNode> Categories { get; } = new();

        /// <summary>Applies the filter the invoked <paramref name="node" /> stands for. Category headers and the "All" value clear the filter.</summary>
        /// <param name="node">The invoked node.</param>
        public Task SelectAsync(FilterNode? node)
        {
            if (node is null || !node.IsFilterValue)
                return Task.CompletedTask;

            IReadOnlyDictionary<string, string> filter = string.Equals(node.Value, AllValue, StringComparison.Ordinal)
                ? EmptyFilter
                : new Dictionary<string, string>(StringComparer.Ordinal) { [node.CategoryKey] = node.Value! };
            return _Session.SetTorrentFilterAsync(filter);
        }

        private void OnSessionStateChanged(object? sender, EventArgs e)
        {
            _Dispatcher.Post(SyncToSession);
        }

        private void SyncToSession()
        {
            if (_Session.State == SessionState.Connected)
                Attach(_Session.Torrents);
            else
                Detach();
        }

        private void Attach(TorrentMonitor? monitor)
        {
            if (monitor is null || ReferenceEquals(monitor, _Attached))
                return;

            Detach();
            _Attached = monitor;
            monitor.Refreshed += OnMonitorRefreshed;
            _ = RefreshTreeAsync();
        }

        private void Detach()
        {
            if (_Attached != null)
            {
                _Attached.Refreshed -= OnMonitorRefreshed;
                _Attached = null;
            }
            Categories.Clear();
        }

        private void OnMonitorRefreshed(object? sender, EventArgs e)
        {
            _ = RefreshTreeAsync();
        }

        private async Task RefreshTreeAsync()
        {
            if (!_Session.Supports(GetFilterTreeRequest.MethodName))
                return;
            if (Interlocked.Exchange(ref _Refreshing, 1) == 1)
                return;

            try
            {
                IReadOnlyDictionary<string, IReadOnlyList<Filter>> tree = await _Session.GetFilterTreeAsync()
                    .ConfigureAwait(false);
                _Dispatcher.Post(() => MergeTree(tree));
            }
#pragma warning disable CA1031 // A transient tree fetch failure is retried on the next poll; a real disconnect surfaces through the session state.
            catch (Exception)
            { }
#pragma warning restore CA1031
            finally
            {
                Interlocked.Exchange(ref _Refreshing, 0);
            }
        }

        private void MergeTree(IReadOnlyDictionary<string, IReadOnlyList<Filter>> tree)
        {
            if (_Session.State != SessionState.Connected)
                return;

            foreach (string category in tree.Keys.OrderBy(CategoryOrder))
                MergeValues(GetOrAddCategory(category), category, tree[category]);

            for (int i = Categories.Count - 1; i >= 0; i--)
            {
                if (!tree.ContainsKey(Categories[i].CategoryKey))
                    Categories.RemoveAt(i);
            }
        }

        private FilterNode GetOrAddCategory(string category)
        {
            FilterNode? existing = Categories.FirstOrDefault(c => string.Equals(c.CategoryKey, category, StringComparison.Ordinal));
            if (existing != null)
                return existing;

            var node = new FilterNode(category, CategoryHeader(category));
            int rank = CategoryOrder(category);
            int at = 0;
            while (at < Categories.Count && CategoryOrder(Categories[at].CategoryKey) < rank)
                at++;
            Categories.Insert(at, node);
            return node;
        }

        private static void MergeValues(FilterNode categoryNode, string category, IReadOnlyList<Filter> values)
        {
            ObservableCollection<FilterNode> children = categoryNode.Children;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (!values.Any(v => string.Equals(v.Value, children[i].Value, StringComparison.Ordinal)))
                    children.RemoveAt(i);
            }

            for (int i = 0; i < values.Count; i++)
            {
                Filter filter = values[i];
                FilterNode? existing = children.FirstOrDefault(c => string.Equals(c.Value, filter.Value, StringComparison.Ordinal));
                if (existing is null)
                {
                    children.Insert(Math.Min(i, children.Count), new FilterNode(category, filter.Value, ValueDisplay(category, filter.Value), filter.Count ?? 0));
                } else
                {
                    existing.Count = filter.Count ?? 0;
                    int current = children.IndexOf(existing);
                    if (current != i && i < children.Count)
                        children.Move(current, i);
                }
            }
        }

        private static string CategoryHeader(string category) => category switch {
            "state" => "States",
            "tracker_host" => "Trackers",
            "owner" => "Owner",
            "label" => "Labels",
            _ => category
        };

        private static string ValueDisplay(string category, string value)
        {
            if (string.IsNullOrEmpty(value))
                return "None";
            if (string.Equals(category, "owner", StringComparison.Ordinal) && string.Equals(value, "localclient", StringComparison.Ordinal))
                return "Admin";
            return value;
        }

        private static int CategoryOrder(string category) => category switch {
            "state" => 0,
            "tracker_host" => 1,
            "owner" => 2,
            "label" => 3,
            _ => 4
        };

        private const string AllValue = "All";
        private static readonly IReadOnlyDictionary<string, string> EmptyFilter = new Dictionary<string, string>(StringComparer.Ordinal);

        private readonly DelugeSession _Session;
        private readonly IUIDispatcher _Dispatcher;
        private TorrentMonitor? _Attached;
        private int _Refreshing;
    }
}
