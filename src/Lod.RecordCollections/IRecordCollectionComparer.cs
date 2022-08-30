namespace System.Collections;

/// <summary>
/// A static utility class containing methods for comparing value collections.
/// </summary>
public interface IRecordCollectionComparer : IEqualityComparer, IEqualityComparer<IReadOnlyRecordCollection>
{
    /// <summary>
    /// Gets the hashcode of the specified <paramref name="collection"/> elements.
    /// This method can be expensive based on the size of the collection.
    /// </summary>
    /// <param name="collection">The collection whose elements should be hashed.</param>
    /// <param name="startingHash">The starting base hash to calculate the hash against.</param>
    /// <param name="rollovers">The number of times the hash has exceeded <see cref="int.MaxValue"/>.</param>
    /// <returns>The hash of the collection elements.</returns>
    public int GetHashCode(IReadOnlyRecordCollection? collection, int startingHash, out int rollovers);

    /// <summary>
    /// Indicates whether a collection is equal to another object of the same type.
    /// </summary>
    /// <param name="x">The first collection to compare.</param>
    /// <param name="y">The second collection to compare.</param>
    public bool Equals(IReadOnlyRecordCollection? x, object? y);
}
