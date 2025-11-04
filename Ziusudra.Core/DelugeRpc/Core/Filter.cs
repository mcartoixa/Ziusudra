using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Represents a filter.</summary>
    public class Filter:
        IComparable<Filter>,
        IEquatable<Filter>
    {

        [SetsRequiredMembers]
        internal Filter(string category, ICollection filter)
        {
            Category = category;

            Debug.Assert(filter.Count == 2);
            var enumerator = filter.GetEnumerator();
            int i = -1;
            while (enumerator.MoveNext())
            {
                i++;
                if (i == 0)
                {
                    if (enumerator.Current is not string v)
                        throw new ArgumentException(nameof(filter));
                    Value = v;
                } else
                {
                    if (Array.IndexOf(_IntegerTypes, Type.GetTypeCode(enumerator.Current.GetType())) < 0)
                        throw new ArgumentException(nameof(filter));
                    Count = Convert.ToInt32(enumerator.Current);
                }

            }
        }

        public override bool Equals(object? obj) => Equals(obj as Filter);

        public bool Equals(Filter? other)
        {
            if (other is null)
                return false;
            return Category.Equals(other.Category) && Value.Equals(other.Value);
        }

        public override int GetHashCode() => Tuple.Create(Category, Value).GetHashCode();

        int IComparable<Filter>.CompareTo(Filter? other)
        {
            if (other is null)
                return 1;
            return ((IComparable)Tuple.Create(Category, Value)).CompareTo(Tuple.Create(other.Category, other.Value));
        }

        public static bool operator ==(Filter? filter1, Filter? filter2)
        {
            if (filter1 is null)
                return filter2 is null;
            return filter1.Equals(filter2);
        }

        public static bool operator !=(Filter? filter1, Filter? filter2)
        {
            if (filter1 is null)
                return filter2 is not null;
            return !filter1.Equals(filter2);
        }

        public string Category { get; set; }

        public required string Value { get; set; }

        public int? Count { get; set; }

        private static readonly TypeCode[] _IntegerTypes = {
            TypeCode.SByte,
            TypeCode.Int16,
            TypeCode.Int32,
            TypeCode.Int64
        };
    }
}
