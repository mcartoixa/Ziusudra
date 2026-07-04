using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Represents a filter.</summary>
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Ordering is exposed only through IComparable for display sorting; comparison operators are not meaningful on a filter.")]
    public class Filter:
        IComparable<Filter>,
        IEquatable<Filter>
    {

        [SetsRequiredMembers]
        internal Filter(string category, ICollection filter)
        {
            Category = category;

            if (filter.Count != 2)
                throw new ArgumentException(SR.Filter_InvalidFilterData, nameof(filter));

            object?[] items = new object?[2];
            filter.CopyTo(items, 0);

            if (items[0] is not string value)
                throw new ArgumentException(SR.Filter_InvalidFilterData, nameof(filter));
            Value = value;

            object? count = items[1];
            if (count is null || Array.IndexOf(_IntegerTypes, Type.GetTypeCode(count.GetType())) < 0)
                throw new ArgumentException(SR.Filter_InvalidFilterData, nameof(filter));
            Count = Convert.ToInt32(count, CultureInfo.InvariantCulture);
        }

        /// <summary>Determines whether the specified object is equal to the current filter.</summary>
        /// <param name="obj">The object to compare with the current filter.</param>
        /// <returns><c>true</c> if the specified object is equal to the current filter; or else <c>false</c>.</returns>
        public override bool Equals(object? obj) => Equals(obj as Filter);

        /// <summary>Determines whether the specified filter is equal to the current filter.</summary>
        /// <param name="other">The filter to compare with the current filter.</param>
        /// <returns><c>true</c> if the specified filter is equal to the current filter; or else <c>false</c>.</returns>
        public bool Equals(Filter? other)
        {
            if (other is null)
                return false;
            return Category.Equals(other.Category, StringComparison.Ordinal) && Value.Equals(other.Value, StringComparison.Ordinal);
        }

        /// <summary>Returns the hash code for the current filter.</summary>
        /// <returns>The hash code for the current filter.</returns>
        public override int GetHashCode() => Tuple.Create(Category, Value).GetHashCode();

        int IComparable<Filter>.CompareTo(Filter? other)
        {
            if (other is null)
                return 1;
            return ((IComparable)Tuple.Create(Category, Value)).CompareTo(Tuple.Create(other.Category, other.Value));
        }

        /// <summary>Determines whether two filters are equal.</summary>
        /// <param name="filter1">The first filter to compare.</param>
        /// <param name="filter2">The second filter to compare.</param>
        /// <returns><c>true</c> if the filters are equal; or else <c>false</c>.</returns>
        public static bool operator ==(Filter? filter1, Filter? filter2)
        {
            if (filter1 is null)
                return filter2 is null;
            return filter1.Equals(filter2);
        }

        /// <summary>Determines whether two filters are different.</summary>
        /// <param name="filter1">The first filter to compare.</param>
        /// <param name="filter2">The second filter to compare.</param>
        /// <returns><c>true</c> if the filters are different; or else <c>false</c>.</returns>
        public static bool operator !=(Filter? filter1, Filter? filter2)
        {
            if (filter1 is null)
                return filter2 is not null;
            return !filter1.Equals(filter2);
        }

        /// <summary>Gets or sets the category of the filter.</summary>
        public string Category { get; init; }

        /// <summary>Gets or sets the value of the filter.</summary>
        public required string Value { get; init; }

        /// <summary>Gets or sets the number of torrents matching the filter.</summary>
        public int? Count { get; set; }

        private static readonly TypeCode[] _IntegerTypes = {
            TypeCode.SByte,
            TypeCode.Int16,
            TypeCode.Int32,
            TypeCode.Int64
        };
    }
}
