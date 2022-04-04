namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed set of objects that can be accessed by index.
    /// Provides methods to search, sort, and manipulate sets.
    /// Record sets support value based comparison.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <remarks>Uses an underlying collection of <see cref="HashSet{T}"/>.</remarks>
    public record RecordSet<T> : RecordCollection<T>, IReadOnlyCollection<T>, ISet<T>
        where T : IEquatable<T>
    {
        #region Properties

        /// <summary>
        /// Gets a factory for instantiating a new instance of the underlying collection.
        /// </summary>
        protected override Func<int, ICollection<T>> CollectionFactory => count =>
        {
#if NET48_OR_GREATER || NET6_0_OR_GREATER
            return new HashSet<T>(count);
#else
            return new HashSet<T>();
#endif
        };

        /// <summary>
        /// Gets the underlying set.
        /// </summary>
        protected virtual HashSet<T> Set => (HashSet<T>)Collection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public RecordSet() : base(new HashSet<T>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying set.
        /// </summary>
        /// <param name="hashSet">An existing <see cref="HashSet{T}"/> to use as the underlying collection.</param>
        public RecordSet(HashSet<T> hashSet) : base(hashSet) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        public RecordSet(IEnumerable<T> collection) : base(new HashSet<T>(collection)) { }

#if NET48_OR_GREATER || NET6_0_OR_GREATER

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new set can initially store.</param>
        public RecordSet(int capacity) : base(new HashSet<T>(capacity)) { }

#endif

        #endregion

        #region Methods

        #endregion

        #region ISet<T>

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        public virtual new bool Add(T item) => Set.Add(item);

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the specified collection, or in both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual void UnionWith(IEnumerable<T> other) => Set.UnionWith(other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual void IntersectWith(IEnumerable<T> other) => Set.IntersectWith(other);

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        public virtual void ExceptWith(IEnumerable<T> other) => Set.ExceptWith(other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual void SymmetricExceptWith(IEnumerable<T> other) => Set.SymmetricExceptWith(other);

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool IsSubsetOf(IEnumerable<T> other) => Set.IsSubsetOf(other);

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool IsSupersetOf(IEnumerable<T> other) => Set.IsSupersetOf(other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool IsProperSupersetOf(IEnumerable<T> other) => Set.IsProperSupersetOf(other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool IsProperSubsetOf(IEnumerable<T> other) => Set.IsProperSubsetOf(other);

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool Overlaps(IEnumerable<T> other) => Set.Overlaps(other);

        /// <summary>
        ///  Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public virtual bool SetEquals(IEnumerable<T> other) => Set.SetEquals(other);

        #endregion

        #region Operators

        public static implicit operator RecordSet<T>(HashSet<T> set) => set != null ? new(set) : null!;

        public static implicit operator HashSet<T>(RecordSet<T> set) => set?.Set!;

        #endregion
    }
}
