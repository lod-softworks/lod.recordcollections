using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed dictionary of objects that can be accessed by index.
    /// Provides methods to search, sort, and manipulate dictionarys.
    /// Record dictionaries support value based comparison of dictionary data.
    /// </summary>
    /// <typeparam name="TKey">The type of elements in the dictionary.</typeparam>
    /// <remarks>Uses an underlying collection of <see cref="Dictionary{TKey, TValue}"/>.</remarks>
    public record RecordDictionary<TKey, TValue> : RecordCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary, IDictionary<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        #region Properties

        /// <summary>
        /// Gets a factory for instantiating a new instance of the underlying collection.
        /// </summary>
        protected override Func<int, ICollection<KeyValuePair<TKey, TValue>>> CollectionFactory => count => new Dictionary<TKey, TValue>(count);

        /// <summary>
        /// Gets the underlying dictionary.
        /// </summary>
        protected virtual Dictionary<TKey, TValue> Dictionary => (Dictionary<TKey, TValue>)Collection;

        /// <summary>
        /// Gets the underlying legacy dictionary.
        /// </summary>
        protected virtual IDictionary LegacyDictionary => Dictionary;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize => LegacyDictionary.IsFixedSize;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// <param name="key">The key of the value to get or set.</param>
        public virtual TValue this[TKey key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public RecordDictionary() : base(new Dictionary<TKey, TValue>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that uses the specified underlying dictionary.
        /// </summary>
        /// <param name="dictionary">An existing <see cref="RecordDictionary{TKey, TValue}"/> to use as the underlying collection.</param>
        public RecordDictionary(Dictionary<TKey, TValue> dictionary) : base(dictionary) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new dictionary.</param>
        public RecordDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection?.ToDictionary(kv => kv.Key, kv => kv.Value)!) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDictionary{TKey, TValue}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new dictionary can initially store.</param>
        public RecordDictionary(int capacity) : base(new Dictionary<TKey, TValue>(capacity)) { }

        #endregion

        #region Methods

        #endregion

        #region IDictionary

        ICollection IDictionary.Keys => LegacyDictionary.Keys;

        ICollection IDictionary.Values => LegacyDictionary.Values;

        object? IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value!;
        }

        /// <summary>
        /// Adds an object to the end of the dictionary.
        /// </summary>
        /// <param name="key">The key which identifies the specified <paramref name="value"/> within the dictionary.</param>
        /// <param name="value">The object to be added to the end of the dictionary. The value can be null for reference types.</param>
        public virtual void Add(object key, object? value) => LegacyDictionary.Add(key, value);

        /// <summary>
        /// Determines whether the dictionary contains a specific value.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        public virtual bool Contains(object key) => LegacyDictionary.Contains(key);

        /// <summary>
        /// Removes the first occurrence of a specific object from the dictionary.
        /// </summary>
        /// <param name="key">The key to remove from the dictionary.</param>
        public virtual void Remove(object key) => LegacyDictionary.Remove(key);

        IDictionaryEnumerator IDictionary.GetEnumerator() => LegacyDictionary.GetEnumerator();

        #endregion

        #region IDictionary<TKey, TValue>

        public ICollection<TKey> Keys => Dictionary.Keys;

        public ICollection<TValue> Values => Dictionary.Values;

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentException">The key is null or already exists in the collection.</exception>
        public virtual void Add(TKey key, TValue value) => Dictionary.Add(key, value);

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <exception cref="ArgumentNullException">Thrown when the key is null.</exception>
        public virtual bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when the key is null.</exception>
        public virtual bool Remove(TKey key) => Dictionary.Remove(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        //  if the key is found; otherwise, the default value for the type of the value parameter.
        //  This parameter is passed uninitialized.
        //  </param>
        /// <returns>
        /// true if the dictionary contains an element with the specified key; otherwise, false.
        //  </returns>
        public virtual bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value!);

        #endregion

        #region Operators

        public static implicit operator RecordDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) => dictionary != null ? new(dictionary) : null!;

        public static implicit operator Dictionary<TKey, TValue>(RecordDictionary<TKey, TValue> dictionary) => dictionary?.Dictionary!;

        #endregion
    }
}
