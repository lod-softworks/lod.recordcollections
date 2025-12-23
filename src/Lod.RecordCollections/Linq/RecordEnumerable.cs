namespace System.Linq;

/// <summary>
/// A static utility exposing queries for <see cref="IEnumerable{T}"/>s for record-type collections similar to <see cref="Enumerable"/>.
/// </summary>
public static class RecordEnumerable
{
    /// <summary>
    /// Creates a <see cref="RecordList{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The record element type.</typeparam>
    /// <param name="enumerable">The enumerable sequence of records.</param>
    /// <returns>A <see cref="RecordList{T}"/> that contains the record elements from the input <paramref name="enumerable"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
    public static RecordList<T> ToRecordList<T>(this IEnumerable<T> enumerable) where T : IEquatable<T> =>
        enumerable != null ? [.. enumerable] : throw new ArgumentNullException(nameof(enumerable));

    /// <summary>
    /// Creates a <see cref="RecordDictionary{TKey, TValue}"/> according to a specified key selector delegate.
    /// </summary>
    /// <typeparam name="TSource">The record element type.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <param name="enumerable">The enumerable sequence of records.</param>
    /// <param name="keySelector">The delegate function which identifies the key for each element.</param>
    /// <returns>A <see cref="RecordDictionary{TKey, TValue}"/> containing the sequence keys and elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> or <paramref name="keySelector"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="keySelector"/> produces dulicate keys for two or more elements.</exception>
    public static RecordDictionary<TKey, TSource> ToRecordDictionary<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector)
        where TKey : notnull
        where TSource : IEquatable<TSource>
    {
        if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        return new RecordDictionary<TKey, TSource>(enumerable.Select(e => new KeyValuePair<TKey, TSource>(keySelector(e), e)));
    }

    /// <summary>
    /// Creates a <see cref="RecordDictionary{TKey, TValue}"/> according to a specified key selector delegate.
    /// </summary>
    /// <typeparam name="TSource">The record element type.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The record value type.</typeparam>
    /// <param name="enumerable">The enumerable sequence of records.</param>
    /// <param name="keySelector">The delegate function which identifies the key for each element.</param>
    /// <param name="elementSelector">The delegate function which identifies the value for each element.</param>
    /// <returns>A <see cref="RecordDictionary{TKey, TValue}"/> containing the sequence keys and elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/>, <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="keySelector"/> produces dulicate keys for two or more elements.</exception>
    public static RecordDictionary<TKey, TValue> ToRecordDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
        where TValue : IEquatable<TValue>
    {
        if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

        return new RecordDictionary<TKey, TValue>(enumerable.Select(e => new KeyValuePair<TKey, TValue>(keySelector(e), elementSelector(e))));
    }

    /// <summary>
    /// Creates a <see cref="RecordSet{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The record element type.</typeparam>
    /// <param name="enumerable">The enumerable sequence of records.</param>
    /// <returns>A <see cref="RecordSet{T}"/> that contains the record elements from the input <paramref name="enumerable"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> is null.</exception>
    public static RecordSet<T> ToRecordSet<T>(this IEnumerable<T> enumerable) where T : IEquatable<T> =>
        enumerable != null ? [.. enumerable] : throw new ArgumentNullException(nameof(enumerable));
}
