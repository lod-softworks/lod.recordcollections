//using System.Diagnostics;
//using System.Reflection;

//namespace System.Collections.Generic;

///// <summary>
///// An abstract base for strongly typed, record based collections.
///// Record collections support value based comparison.
///// </summary>
///// <typeparam name="T">The type of the elements in the colllection.</typeparam>
//// TODO: mark for possible deletion
//internal abstract record RecordCollectionBase<T> : IRecordCollection<T>
//// all elements need to be records or support record like equality
//// no where declaration is specified in order to support dictionaries; however, the base class can only be constructed internally.
//// this will need to be enforeced on each derived class
//{
//    const int DefaultHash = -2018520234;
//    static Lazy<MethodInfo?> CloneMethod { get; } = new(() => typeof(T).GetMethod("<Clone>$", BindingFlags.Instance | BindingFlags.Public));

//    /// <summary>
//    /// Gets the underlying collection container.
//    /// </summary>
//    protected virtual CollectionContainer Container { get; }

//    /// <summary>
//    /// Gets a factory for instantiating a new instance of the underlying collection.
//    /// </summary>
//    protected abstract Func<int, ICollection<T>> CollectionFactory { get; }

//    /// <summary>
//    /// Gets the underlying collection.
//    /// </summary>
//    protected virtual ICollection<T> Collection => Container.Collection;

//    /// <summary>
//    /// Gets the underlying legacy collection.
//    /// </summary>
//    protected virtual ICollection LegacyCollection => (ICollection)Collection;

//    /// <summary>
//    /// Gets a value indicaintg whether the underlying collection is read-only.
//    /// </summary>
//    public virtual bool IsReadOnly => Collection.IsReadOnly;

//    /// <summary>
//    /// Gets the number of elements contained in the <see cref="RecordCollection{T}"/>.
//    /// </summary>
//    public virtual int Count => Collection.Count;

//    /// <summary>
//    /// Instantiates a new instance of the record using values from an existing collection.
//    /// </summary>
//    /// <param name="collection"/>
//    protected RecordCollectionBase(RecordCollectionBase<T> collection)
//    {
//        if (collection == null) throw new ArgumentNullException(nameof(collection));

//        ICollection<T> newCollection = CollectionFactory(collection.Count);

//        if (!newCollection.IsReadOnly)
//        {
//            foreach (T element in collection.Collection)
//            {
//                T? newElement = CloneElement(element);

//                newCollection.Add((!ReferenceEquals(newElement, default(T)) ? newElement : element)!);
//            }
//        }
//        else if (newCollection is IList newList && collection.Collection is IList oldList)
//        {
//            // loop through not using the Add function to support read-only lists (i.e. arrays)
//            for (int i = 0; i < oldList.Count; i++)
//            {
//                T? element = (T?)oldList[i];
//                T? newElement = CloneElement(element);

//                newList[i] = !ReferenceEquals(newElement, default(T)) ? newElement : element;
//            }
//        }
//        else
//        {
//            newCollection = collection.Collection;
//        }

//        Container = new(newCollection);
//    }

//    /// <summary>
//    /// Instantiates the base record collection.
//    /// </summary>
//    /// <param name="collection">The underlying collection.</param>
//    /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
//    internal RecordCollectionBase(ICollection<T> collection) => Container = new(collection ?? throw new ArgumentNullException(nameof(collection)));

//    /// <summary>
//    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instance.
//    /// </summary>
//    /// <param name="other">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
//    public virtual bool Equals(RecordCollectionBase<T>? other) => Equals(this, other);

//    /// <summary>
//    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instances underlying collection.
//    /// </summary>
//    /// <param name="other">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection's elements.</return>
//    public virtual bool Equals(ICollection<T>? other) => Equals(this, other);

//    /// <summary>
//    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
//    /// </summary>
//    /// <param name="left">The original collection to compare the other collection to.</param>
//    /// <param name="right">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
//    public virtual bool Equals(RecordCollectionBase<T>? left, RecordCollectionBase<T>? right) =>
//        Equals((ICollection<T>?)left, right);

//    /// <summary>
//    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
//    /// </summary>
//    /// <param name="left">The original collection to compare the other collection to.</param>
//    /// <param name="right">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
//    protected virtual bool Equals(ICollection<T>? left, ICollection<T>? right)
//    {
//        if (left == null && right == null) return true;
//        if (left == null && right != null) return false;
//        if (left != null && right == null) return false;
//        if (ReferenceEquals(left, right)) return true;
//        if (left!.Count != right!.Count) return false;

//        CollectionContainer leftContainer = new(left);
//        CollectionContainer rightContainer = new(right);
//        bool areEqual = leftContainer.GetHashCode() == rightContainer.GetHashCode();

//        return areEqual;
//    }

//    /// <summary>
//    /// Returns a hash of the underlying elements.
//    /// </summary>
//    /// <returns>The hash of the underlying elements.</returns>
//    public override int GetHashCode() => Container.GetHashCode();

//    /// <summary>
//    /// Returns a cloned instance of a specific element.
//    /// </summary>
//    /// <returns/>
//    protected virtual T? CloneElement(T? element)
//    {
//        T? newElement = default;

//        if (element != null && CloneMethod.Value != null)
//        {
//            newElement = (T?)CloneMethod.Value.Invoke(element, null);
//        }

//        return newElement;
//    }

//    #region IRecordCollection

//    /// <summary>
//    /// Returns a value indicating whether an <paramref name="other"/> collection is equal to the current instances underlying collection.
//    /// </summary>
//    /// <param name="other">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection's elements.</return>
//    public virtual bool Equals(IRecordCollection<T>? other) => Equals((ICollection<T>?)this, other);

//    /// <summary>
//    /// Returns a value indicating whether the <paramref name="left"/> collection is equal to the <paramref name="right"/> collection.
//    /// </summary>
//    /// <param name="left">The original collection to compare the other collection to.</param>
//    /// <param name="right">The collection to compare the current collection to.</param>
//    /// <return>True if the underlying collection's elements are equivalent to the current collection.</return>
//    public virtual bool Equals(IRecordCollection<T>? left, IRecordCollection<T>? right) => Equals((ICollection<T>?)left, right);

//    #endregion

//    #region IEnumerable

//    [DebuggerHidden]
//    IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();

//    /// <summary>
//    /// Returns an enumerator that iterates through the collection.
//    /// </summary>
//    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
//    public virtual IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();

//    #endregion

//    #region ICollection

//    [DebuggerHidden]
//    bool ICollection.IsSynchronized => LegacyCollection.IsSynchronized;

//    [DebuggerHidden]
//    object ICollection.SyncRoot => LegacyCollection.SyncRoot;

//    [DebuggerHidden]
//    void ICollection.CopyTo(Array array, int index) => LegacyCollection.CopyTo(array, index);

//    #endregion

//    #region ICollection<T>

//    /// <summary>
//    /// Adds an object to the end of the collection.
//    /// </summary>
//    /// <param name="value">The object to be added to the end of the collection.</param>
//    /// <exception cref="NotSupportedException">Thrown when the collection is read-only.</exception>
//    public virtual void Add(T value) => Collection.Add(value);

//    /// <summary>
//    /// Removes all items from the collection.
//    /// </summary>
//    /// <exception cref="NotSupportedException">Thrown when the collection is read-only.</exception>
//    public virtual void Clear() => Collection.Clear();

//    /// <summary>
//    /// Determines whether the collection contains a specific value.
//    /// </summary>
//    /// <param name="value">The object to locate in the collection.</param>
//    public virtual bool Contains(T value) => Collection.Contains(value);

//    /// <summary>
//    /// Removes the first occurrence of a specific object from the collection.
//    /// </summary>
//    /// <param name="value">The object to remove from the collection.</param>
//    /// <exception cref="NotSupportedException">Thrown when the collection is read-only.</exception>
//    public virtual bool Remove(T value) => Collection.Remove(value);

//    /// <summary>
//    /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
//    /// </summary>
//    /// <param name="array">
//    /// The one-dimensional System.Array that is the destination of the elements copied
//    /// from the collection. The System.Array must have zero-based indexing.
//    /// </param>
//    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
//    /// <exception cref="ArgumentNullException"><paramref name="arrayIndex"/> is less than zero.</exception>
//    /// <exception cref="ArgumentException">
//    /// The number of elements in the source collection is greater than the available space from
//    /// <paramref name="arrayIndex"/> to the end of the destination array.
//    /// </exception>
//    public virtual void CopyTo(T[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

//    #endregion

//    #region IEquatable

//    [DebuggerHidden]
//    bool IEquatable<ICollection<T>>.Equals(ICollection<T>? other) => Equals(other);

//    #endregion

//    #region IEqualityComparer

//    [DebuggerHidden]
//    bool IEqualityComparer<ICollection<T>>.Equals(ICollection<T>? x, ICollection<T>? y) => Equals(x, y);

//    [DebuggerHidden]
//    int IEqualityComparer<ICollection<T>>.GetHashCode(ICollection<T>? obj) => obj?.GetHashCode() ?? DefaultHash;

//    #endregion

//    #region IStructuralEquatable

//    [DebuggerHidden]
//    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) => comparer.Equals(this, other);

//    [DebuggerHidden]
//    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => comparer.GetHashCode(this);

//    #endregion

//    #region IStructuralComparable

//    [DebuggerHidden]
//    int IStructuralComparable.CompareTo(object? other, IComparer comparer) => comparer.Compare(this, other);

//    #endregion

//    #region Support Types

//    /// <summary>
//    /// A container for the underlying collection which exposes the collection but modifies it's hash lookup.
//    /// </summary>
//    protected class CollectionContainer : IEquatable<CollectionContainer>
//    {
//        /// <summary>
//        /// Gets the underlying collection
//        /// </summary>
//        public virtual ICollection<T> Collection { get; }

//        /// <summary>
//        /// Instantiates an instance of the container collection with the container to be wrapped.
//        /// </summary>
//        /// <param name="collection">The container to wrap.</param>
//        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
//        public CollectionContainer(ICollection<T> collection) => Collection = collection ?? throw new ArgumentNullException(nameof(collection));

//        /// <summary>
//        /// Gets the hashcode of the underlying elements.
//        /// This method can be expensive based on the size of the collection.
//        /// Derived classes should optimize this method based on the underlying collection.
//        /// </summary>
//        /// <returns>The hash of the collection elements.</returns>
//        public override int GetHashCode()
//        {
//            int hash = DefaultHash;

//            unchecked
//            {
//                if (Collection is IList list)
//                {
//                    // order is important
//                    for (int i = list.Count - 1; i >= 0; i--)
//                    {
//                        hash += (list[i]?.GetHashCode() ?? default) ^ i;
//                    }
//                }
//                else
//                {
//                    // order is not important
//                    foreach (T item in Collection)
//                    {
//                        hash += (item?.GetHashCode() ?? default) ^ 3;
//                    }
//                }
//            }

//            return hash;
//        }

//        /// <summary>
//        /// Indicates whether the current object is equal to another object of the same type.
//        /// </summary>
//        /// <param name="other">An object to compare with this object.</param>
//        public override bool Equals(object? other) => other is CollectionContainer collection && Equals(collection);

//        /// <summary>
//        /// Indicates whether the current object is equal to another object of the same type.
//        /// </summary>
//        /// <param name="other">An object to compare with this object.</param>
//        public virtual bool Equals(CollectionContainer? other)
//        {
//            bool areEqual = other != null
//                && Collection.Count == other.Collection.Count
//                && GetHashCode() == other.GetHashCode();

//            return areEqual;
//        }
//    }

//    #endregion
//}
