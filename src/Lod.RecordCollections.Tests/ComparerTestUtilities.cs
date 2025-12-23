using System;
using System.Threading;

namespace System.Collections.Tests;

internal static class ComparerTestUtilities
{
    private static readonly object ComparerLock = new();

    public static IDisposable OverrideDefaultComparer(IRecordCollectionComparer comparer) =>
        new ComparerScope(comparer);

    private sealed class ComparerScope : IDisposable
    {
        private readonly IRecordCollectionComparer _original;
        private bool _disposed;

        public ComparerScope(IRecordCollectionComparer comparer)
        {
            Monitor.Enter(ComparerLock);
#pragma warning disable CS0618
            _original = RecordCollectionComparer.Default;
            RecordCollectionComparer.Default = comparer;
#pragma warning restore CS0618
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
#pragma warning disable CS0618
            RecordCollectionComparer.Default = _original;
#pragma warning restore CS0618
            Monitor.Exit(ComparerLock);
        }
    }
}
