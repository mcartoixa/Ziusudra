using System.ComponentModel;
using System.Diagnostics;

namespace Ziusudra.Desktop.ViewModel
{

    public class FilterCategory:
        BindingList<Filter>,
        IComparable<FilterCategory>,
        IEquatable<FilterCategory>
    {

        private FilterCategory()
        { }

        public FilterCategory(string category)
        {
            Category = category;
        }

        public FilterCategory(IEnumerable<Filter> filters):
            base(filters.ToList())
        {
            Category = filters.FirstOrDefault()?.Category ?? string.Empty;

            Debug.Assert(filters.All(f => f.Category == Category));
            if (filters.Any(f => f.Category != Category))
                throw new ArgumentException($"All filters should be of the same category \"{Category}\"", nameof(filters));
        }

        public override bool Equals(object? obj) => Equals(obj as FilterCategory);

        public bool Equals(FilterCategory? other)
        {
            if (other is null)
                return false;
            return Category.Equals(other.Category);
        }

        public override int GetHashCode() => Category.GetHashCode();

        int IComparable<FilterCategory>.CompareTo(FilterCategory? other)
        {
            if (other is null)
                return 1;
            return Category.CompareTo(other.Category);
        }

        public static bool operator ==(FilterCategory? fc1, FilterCategory? fc2)
        {
            if (fc1 is null)
                return fc2 is null;
            return fc1.Equals(fc2);
        }

        public static bool operator !=(FilterCategory? fc1, FilterCategory? fc2)
        {
            if (fc1 is null)
                return fc2 is not null;
            return !fc1.Equals(fc2);
        }

        public string Category { get; private set; } = string.Empty;
    }
}
