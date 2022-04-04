using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed array of objects that can be accessed by index.
    /// Provides methods to search, sort, and manipulate arrays.
    /// Record arrays support value based comparison.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <remarks>Uses an underlying collection of <see cref="System.Array"/>.</remarks>
    internal record RecordArray<T> : RecordCollection<T>, IList, IReadOnlyList<T> // TODO: RecordArray cannot be deserialized by common means.
        where T : IEquatable<T>
    {
        #region Properties

        /// <summary>
        /// Gets a factory for instantiating a new instance of the underlying collection.
        /// </summary>
        protected override Func<int, ICollection<T>> CollectionFactory => length => new T[length];

        /// <summary>
        /// Gets the underlying array.
        /// </summary>
        protected virtual T[] Array => (T[])Collection;

        /// <summary>
        /// Gets the underlying list.
        /// </summary>
        protected virtual IList LegacyList => Array;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize => Array.IsFixedSize;

        /// <summary>
        /// Gets the total number of elements in all the dimensions of the array.
        /// </summary>
        public virtual int Length => Array.Length;

        /// <summary>
        /// Gets a 64-bit integer that represents the total number of elements in all the dimensions of the array.
        /// </summary>
        public virtual long LongLength => Array.LongLength;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public virtual T this[int index]
        {
            get => Array[index];
            set => Array[index] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordArray{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public RecordArray() : base(new T[0]) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordArray{T}"/> class that uses the specified underlying array.
        /// </summary>
        /// <param name="array">An existing <see cref="System.Array"/> to use as the underlying collection.</param>
        public RecordArray(T[] array) : base(array) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordArray{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new array.</param>
        public RecordArray(IEnumerable<T> collection) : base(collection?.ToArray()!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordArray{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new array can initially store.</param>
        public RecordArray(int capacity) : base(new T[capacity]) { }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the array contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the array.</param>
        public virtual int IndexOf(T value) => LegacyList.IndexOf(value);

        #endregion

        #region IList

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value!;
        }

        /// <summary>
        /// Determines whether the array contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the array.</param>
        public virtual bool Contains(object? value) => LegacyList.Contains(value);

        /// <summary>
        /// Determines whether the array contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the array.</param>
        public virtual int IndexOf(object? value) => LegacyList.IndexOf(value);
        
        int IList.Add(object? value) => LegacyList.Add(value);

        void IList.Insert(int index, object? value) => LegacyList.Insert(index, value);

        void IList.Remove(object? value) => LegacyList.Remove(value);

        void IList.RemoveAt(int index) => LegacyList.RemoveAt(index);

        #endregion

        #region Operators

        public static implicit operator RecordArray<T>(T[] array) => array != null ? new(array) : null!;

        public static implicit operator T[](RecordArray<T> array) => array?.Array!;

        #endregion
    }
}
