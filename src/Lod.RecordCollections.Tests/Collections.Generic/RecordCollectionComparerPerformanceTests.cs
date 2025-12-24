using System.Diagnostics;

namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public sealed class RecordCollectionComparerPerformanceTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void SetUp()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = new RecordCollectionComparer();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private static bool ShouldRunStress() =>
        string.Equals(Environment.GetEnvironmentVariable("RUN_RECORDCOLLECTION_STRESS"), "1", StringComparison.OrdinalIgnoreCase);

    private static int GetSizeOrDefault(int @default)
    {
        string? raw = Environment.GetEnvironmentVariable("RECORDCOLLECTION_STRESS_SIZE");
        return int.TryParse(raw, out int size) && size > 0 ? size : @default;
    }

    [TestMethod]
    public void Stress_RecordList_Int32_Compare_And_Time()
    {
        if (!ShouldRunStress()) return;

        int n = GetSizeOrDefault(@default: 1_000_000);

        RecordList<int> left = new(n);
        RecordList<int> right = new(n);

        for (int i = 0; i < n; i++)
        {
            left.Add(i);
            right.Add(i);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordList<int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");

        sw.Restart();
        _ = left.GetHashCode();
        _ = right.GetHashCode();
        sw.Stop();
        TestContext.WriteLine($"RecordList<int>.GetHashCode x2 (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordSet_Int32_Compare_And_Time()
    {
        if (!ShouldRunStress()) return;

        int n = GetSizeOrDefault(@default: 1_000_000);

        RecordSet<int> left = new(capacity: n);
        RecordSet<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            left.Add(i);
            right.Add(i);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordSet<int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");

        sw.Restart();
        _ = left.GetHashCode();
        _ = right.GetHashCode();
        sw.Stop();
        TestContext.WriteLine($"RecordSet<int>.GetHashCode x2 (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordDictionary_Int32_Int32_Compare_And_Time()
    {
        if (!ShouldRunStress()) return;

        // Dictionaries are heavier per element; keep default slightly lower unless overridden.
        int n = GetSizeOrDefault(@default: 500_000);

        RecordDictionary<int, int> left = new(capacity: n);
        RecordDictionary<int, int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            left[i] = i;
            right[i] = i;
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordDictionary<int,int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");

        sw.Restart();
        _ = left.GetHashCode();
        _ = right.GetHashCode();
        sw.Stop();
        TestContext.WriteLine($"RecordDictionary<int,int>.GetHashCode x2 (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordQueue_Int32_Compare_And_Time()
    {
        if (!ShouldRunStress()) return;

        int n = GetSizeOrDefault(@default: 1_000_000);

        RecordQueue<int> left = new(capacity: n);
        RecordQueue<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            left.Enqueue(i);
            right.Enqueue(i);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordQueue<int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordStack_Int32_Compare_And_Time()
    {
        if (!ShouldRunStress()) return;

        int n = GetSizeOrDefault(@default: 1_000_000);

        RecordStack<int> left = new(capacity: n);
        RecordStack<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            left.Push(i);
            right.Push(i);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordStack<int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }
}

