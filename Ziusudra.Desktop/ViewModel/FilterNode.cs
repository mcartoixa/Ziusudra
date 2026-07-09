using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>A node in the filter sidebar: either a category header (State, Trackers, …) or a selectable filter value under it.</summary>
    public sealed partial class FilterNode:
        ObservableObject
    {

        /// <summary>Create a category header node.</summary>
        /// <param name="categoryKey">The daemon's filter category key (for example <c>state</c>).</param>
        /// <param name="header">The header shown to the user.</param>
        public FilterNode(string categoryKey, string header)
        {
            CategoryKey = categoryKey;
            Value = null;
            _Display = header;
        }

        /// <summary>Create a selectable filter-value node.</summary>
        /// <param name="categoryKey">The daemon's filter category key (for example <c>state</c>).</param>
        /// <param name="value">The raw filter value passed to the daemon (for example <c>Downloading</c>).</param>
        /// <param name="display">The value shown to the user.</param>
        /// <param name="count">The number of torrents matching the value.</param>
        public FilterNode(string categoryKey, string value, string display, int count)
        {
            CategoryKey = categoryKey;
            Value = value;
            _Display = display;
            _Count = count;
        }

        /// <summary>Gets the daemon's filter category key this node belongs to.</summary>
        public string CategoryKey { get; }

        /// <summary>Gets the raw filter value, or <c>null</c> when this node is a category header.</summary>
        public string? Value { get; }

        /// <summary>Gets a value indicating whether this node selects a filter value (rather than being a category header).</summary>
        public bool IsFilterValue => Value is not null;

        /// <summary>Gets the value nodes under this category header; empty for value nodes.</summary>
        public ObservableCollection<FilterNode> Children { get; } = new();

        /// <summary>The number of torrents matching this value.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Label))]
        private int _Count;

        /// <summary>Gets the text shown for this node: the header for a category, or "value (count)" for a value.</summary>
        public string Label => Value is null ? _Display : $"{_Display} ({Count})";

        private readonly string _Display;
    }
}
