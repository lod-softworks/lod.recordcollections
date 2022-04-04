namespace System.Collections.Generic
{
    /// <summary>
    /// A collection of strongly typed, record-like values.
    /// Record collections support value based comparison.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the colllection.</typeparam>
    public interface IRecordCollection<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
        , IEnumerable<T>, IEnumerable, IEquatable<ICollection<T>>, IEqualityComparer<ICollection<T>>
        , IStructuralEquatable, IStructuralComparable
    {
        /// <summary>
        /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instance.
        /// </summary>
        /// <param name="other">The collection to compare the current collection to.</param>
        /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
        public bool Equals(IRecordCollection<T>? other);

        /// <summary>
        /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
        /// </summary>
        /// <param name="left">The original collection to compare the other collection to.</param>
        /// <param name="right">The collection to compare the current collection to.</param>
        /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
        public bool Equals(IRecordCollection<T>? left, IRecordCollection<T>? right);

        /// <summary>
        /// Returns a hash of the underlying elements.
        /// </summary>
        /// <returns>The hash of the underlying elements.</returns>
        public int GetHashCode();
    }
}
