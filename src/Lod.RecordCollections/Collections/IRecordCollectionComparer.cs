namespace System.Collections;

/// <summary>
/// Defines methods to support the comparison of record collections.
/// </summary>
public interface IRecordCollectionComparer : IEqualityComparer, IEqualityComparer<IReadOnlyRecordCollection>
{
    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public bool Equals(IReadOnlyRecordCollection? x, object? y);
}
