//using System.Reflection;

//namespace System.Collections.Generic
//{
//    /// <summary>
//    /// An abstract base for strongly typed, record based collections.
//    /// Record collections support value based comparison.
//    /// </summary>
//    /// <typeparam name="T">The type of the elements in the colllection.</typeparam>
//    // TODO: mark for possible deletion
//    [DefaultMember("Item")]
//    internal abstract record RecordCollection<T> : RecordCollectionBase<T>
//        where T : IEquatable<T> // all elements need to be records or support record like equality
//    {
//        /// <summary>
//        /// Instantiates the base record collection.
//        /// </summary>
//        /// <param name="collection">The underlying collection.</param>
//        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
//        protected RecordCollection(ICollection<T> collection) : base(collection) { }
//    }
//}
