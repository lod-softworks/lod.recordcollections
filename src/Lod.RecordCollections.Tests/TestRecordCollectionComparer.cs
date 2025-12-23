namespace System.Collections.Tests;

internal sealed class TestRecordCollectionComparer : IRecordCollectionComparer
{
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;

    public bool Equals(IReadOnlyRecordCollection? x, IReadOnlyRecordCollection? y) => ReferenceEquals(x, y);

    public int GetHashCode(IReadOnlyRecordCollection? obj) => obj?.GetHashCode() ?? 0;

    public bool Equals(IReadOnlyRecordCollection? x, object? y) => ReferenceEquals(x, y);
}
