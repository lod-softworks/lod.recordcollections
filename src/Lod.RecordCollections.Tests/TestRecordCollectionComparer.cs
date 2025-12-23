namespace System.Collections.Tests;

internal sealed class TestRecordCollectionComparer : global::System.Collections.IRecordCollectionComparer
{
    public bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;

    public bool Equals(global::System.Collections.IReadOnlyRecordCollection? x, global::System.Collections.IReadOnlyRecordCollection? y) => ReferenceEquals(x, y);

    public int GetHashCode(global::System.Collections.IReadOnlyRecordCollection? obj) => obj?.GetHashCode() ?? 0;

    public bool Equals(global::System.Collections.IReadOnlyRecordCollection? x, object? y) => ReferenceEquals(x, y);
}
