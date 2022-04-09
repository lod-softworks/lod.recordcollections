using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index.
    /// Provides methods to search, sort, and manipulate lists.
    /// Record lists support value based comparison.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <remarks>Uses an underlying collection of <see cref="List{T}"/>.</remarks>
    public record RecordList<T> : RecordCollection<T>, IList<T>, IReadOnlyList<T>, IList
        where T : IEquatable<T>
    {
        #region Properties

        /// <summary>
        /// Gets a factory for instantiating a new instance of the underlying collection.
        /// </summary>
        protected override Func<int, ICollection<T>> CollectionFactory => count => new List<T>(count);

        /// <summary>
        /// Gets the underlying list.
        /// </summary>
        protected virtual List<T> List => (List<T>)Collection;

        /// <summary>
        /// Gets the underlying legacy list.
        /// </summary>
        protected virtual IList LegacyList => List;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize => LegacyList.IsFixedSize;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public virtual T this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public RecordList() : base(new List<T>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSet{T}"/> class that uses the specified underlying list.
        /// </summary>
        /// <param name="list">An existing <see cref="List{T}"/> to use as the underlying collection.</param>
        public RecordList(List<T> list) : base(list) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordList{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public RecordList(IEnumerable<T> collection) : base(new List<T>(collection)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordList{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public RecordList(int capacity) : base(new List<T>(capacity)) { }

        #endregion

        #region Methods

        #endregion

        #region IList

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value!;
        }

        [DebuggerHidden]
        int IList.Add(object value) => LegacyList.Add(value);

        [DebuggerHidden]
        bool IList.Contains(object value) => LegacyList.Contains(value);

        [DebuggerHidden]
        int IList.IndexOf(object value) => LegacyList.IndexOf(value);

        [DebuggerHidden]
        void IList.Insert(int index, object value) => LegacyList.Insert(index, value);

        [DebuggerHidden]
        void IList.Remove(object? value) => LegacyList.Remove(value);

        #endregion

        #region IList<T>

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire System.Collections.Generic.List`1.
        /// </summary>
        /// <param name="item">The object to locate in collection. The value can be null for reference types.</param>
        /// <remarks>The zero-based index of the first occurrence of item within the collection, if found; otherwise, –1.</remarks>
        public virtual int IndexOf(T item) => List.IndexOf(item);

        /// <summary>
        /// Inserts an element into thelist at the specified
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public virtual void Insert(int index, T item) => List.Insert(index, item);

        /// <summary>
        /// Removes the element at the specified index of the list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public virtual void RemoveAt(int index) => List.RemoveAt(index);

        #endregion

        #region Operators

        /// <summary>
        /// Casts the <pararmref name="list"/> to a record list, wrapping the existing list reference.
        /// </summary>
        /// <param name="list">The list to wrap in a record list.</param>
        public static implicit operator RecordList<T>(List<T> list) => list != null ? new(list) : null!;

        /// <summary>
        /// Casts the <pararmref name="list"/> to list, returning the underling list reference.
        /// </summary>
        /// <param name="list">The list to unwrap.</param>
        public static implicit operator List<T>(RecordList<T> list) => list?.List!;

        #endregion
    }
}
