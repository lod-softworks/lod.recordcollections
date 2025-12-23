namespace System.Collections.Generic;

/// <summary>
/// A collection of strongly typed, record-like values.
/// Record collections support value based comparison.
/// </summary>
/// <typeparam name="T">The type of the elements in the colllection.</typeparam>
public interface IRecordCollection<T> : IReadOnlyRecordCollection<T>
    , ICollection, ICollection<T>
    , IEquatable<IRecordCollection<T>>, IEqualityComparer<IRecordCollection<T>>
{ }
