using System.Diagnostics;

namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordQueueTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void SetUp()
    {
        RecordCollectionComparer.Default = new RecordCollectionComparer();
    }

    private static int GetSizeOrDefault(int @default)
    {
        string? raw = Environment.GetEnvironmentVariable("RECORDCOLLECTION_STRESS_SIZE");
        return int.TryParse(raw, out int size) && size > 0 ? size : @default;
    }

    /// <remarks>This test is a sanity check to ensure that the default Queue.Equals behavior is as expected.</remarks>
    [TestMethod]
    public void Queue_SameInts_NotEqualsMatchingQueue()
    {
        // Arrange
        Queue<int> queue1 = new([92, 117, 420]);
        Queue<int> queue2 = new([92, 117, 420]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    [DoNotParallelize]
    public void RecordQueue_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
        RecordCollectionComparer.Default = comparer;

        // Act
        RecordQueue<int> queue = new();

        // Assert
        Assert.AreSame(comparer, queue.Comparer);
    }

    [TestMethod]
    public void RecordQueue_CustomComparerConstructor_UsesProvidedComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();

        // Act
        RecordQueue<int> queue = new(comparer);

        // Assert
        Assert.AreSame(comparer, queue.Comparer);
    }

    [TestMethod]
    public void RecordQueue_Operators_UseTypedEquals()
    {
        OperatorAwareRecordQueue left = new([92, 117]);
        OperatorAwareRecordQueue right = new([92, 117]);

        left.Reset();
        _ = left == right;
        Assert.IsTrue(left.TypedEqualsCalled);
        Assert.IsFalse(left.ObjectEqualsCalled);

        left.Reset();
        _ = left != right;
        Assert.IsTrue(left.TypedEqualsCalled);
        Assert.IsFalse(left.ObjectEqualsCalled);
    }

    [TestMethod]
    public void RecordQueue_SameInts_EqualsMatchingQueue()
    {
        // Arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([92, 117, 420]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_Equals_Matching_SystemQueue()
    {
        // Arrange
        RecordQueue<int> record = new([1, 2, 3]);
        Queue<int> system = new([1, 2, 3]);

        // Act & Assert
        Assert.IsTrue(record.Equals(system));
    }

    [TestMethod]
    public void RecordQueue_SameStrings_EqualsMatchingQueue()
    {
        // Arrange
        RecordQueue<string> queue1 = new(["92", "117", "420"]);
        RecordQueue<string> queue2 = new(["92", "117", "420"]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_SameRecords_EqualsMatchingQueue()
    {
        // Arrange
        RecordQueue<Number> queue1 = new([new Number(92), new Number(117), new Number(420)]);
        RecordQueue<Number> queue2 = new([new Number(92), new Number(117), new Number(420)]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_SameInts_DifferentOrder_NotEqualsSimilarQueue()
    {
        // Arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([420, 117, 92]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_DifferentValues_NotEquals()
    {
        // Arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([92, 117, 421]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_DifferentSizes_NotEquals()
    {
        // Arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([92, 117]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_EmptyVsNonEmpty_NotEquals()
    {
        // Arrange
        RecordQueue<int> queue1 = new();
        RecordQueue<int> queue2 = new([92, 117, 420]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_OneValueDifferent_NotEquals()
    {
        // Arrange
        RecordQueue<int> queue1 = new([1, 2, 3, 4, 5]);
        RecordQueue<int> queue2 = new([1, 2, 99, 4, 5]);

        // Act
        bool areEqual = queue1.Equals(queue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_EnqueueDequeue_OrderPreserved()
    {
        // Arrange
        RecordQueue<int> queue = new();
        queue.Enqueue(92);
        queue.Enqueue(117);
        queue.Enqueue(420);

        // Act
        int first = queue.Dequeue();
        int second = queue.Dequeue();
        int third = queue.Dequeue();

        // Assert
        Assert.AreEqual(92, first);
        Assert.AreEqual(117, second);
        Assert.AreEqual(420, third);
    }

    [TestMethod]
    public void RecordQueue_DeserializedNewtonsoft_EqualsReserialized()
    {
        // Arrange
        RecordQueue<Number> queue = new([new Number(92), new Number(117), new Number(420)]);

        // Act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(queue);
        RecordQueue<Number>? recordQueue = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordQueue<Number>>(json);
        Queue<Number>? systemQueue = Newtonsoft.Json.JsonConvert.DeserializeObject<Queue<Number>>(json);

        // Assert
        Assert.IsNotNull(recordQueue, "Deserialized record queue is null.");
        Assert.IsNotNull(systemQueue, "Deserialized queue is null.");
        Assert.IsTrue(queue.Equals(recordQueue), "Deserialized queue is not equal to the original queue.");
        Assert.HasCount(queue.Count, recordQueue, "Deserialized queue count does not match.");
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordQueue_DeserializedSystemTextJson_EqualsReserialized()
    {
        // Arrange
        RecordQueue<Number> queue = new([new Number(92), new Number(117), new Number(420)]);

        // Act
        string json = System.Text.Json.JsonSerializer.Serialize(queue);
        RecordQueue<Number>? recordQueue = System.Text.Json.JsonSerializer.Deserialize<RecordQueue<Number>>(json);
        Queue<Number>? systemQueue = System.Text.Json.JsonSerializer.Deserialize<Queue<Number>>(json);

        // Assert
        Assert.IsNotNull(recordQueue, "Deserialized record queue is null.");
        Assert.IsNotNull(systemQueue, "Deserialized queue is null.");
        Assert.IsTrue(queue.Equals(recordQueue), "Deserialized queue is not equal to the original queue.");
        Assert.HasCount(queue.Count, recordQueue, "Deserialized queue count does not match.");
    }
#endif

    [TestMethod]
    public void Stress_RecordQueue_Int32_Compare_And_Time()
    {
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
    [RepeatTestMethod(10)]
    public void Stress_RecordQueue_Int32_Random_Compare_And_Time()
    {
        int n = GetSizeOrDefault(@default: 1_000_000);
        Random random = new();

        RecordQueue<int> left = new(capacity: n);
        RecordQueue<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            int value = random.Next();
            left.Enqueue(value);
            right.Enqueue(value);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordQueue<int>.Equals with random values (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordQueue_Int32_Random_OneDifference_NotEquals()
    {
        int n = GetSizeOrDefault(@default: 1_000_000);
        Random random = new();
        List<int> values = new(capacity: n);

        RecordQueue<int> left = new(capacity: n);
        RecordQueue<int> right = new(capacity: n);

        // Add same random values to both collections
        for (int i = 0; i < n; i++)
        {
            int value = random.Next();
            values.Add(value);
            left.Enqueue(value);
            right.Enqueue(value);
        }

        // Rebuild right queue with one difference at a random position
        right = new RecordQueue<int>(capacity: n);
        int differenceIndex = random.Next(0, n);
        int originalValue = values[differenceIndex];
        int differentValue = random.Next();
        // Ensure the different value is actually different
        while (differentValue == originalValue)
        {
            differentValue = random.Next();
        }

        for (int i = 0; i < n; i++)
        {
            if (i == differenceIndex)
            {
                right.Enqueue(differentValue);
            }
            else
            {
                right.Enqueue(values[i]);
            }
        }

        // Act
        bool areEqual = left.Equals(right);

        // Assert
        Assert.IsFalse(areEqual, $"Collections should not be equal with one difference at index {differenceIndex}");
    }

    #region Support Types

    private sealed class OperatorAwareRecordQueue(IEnumerable<int> values) : RecordQueue<int>(values)
    {
        public bool TypedEqualsCalled { get; private set; }
        public bool ObjectEqualsCalled { get; private set; }

        public void Reset()
        {
            TypedEqualsCalled = false;
            ObjectEqualsCalled = false;
        }

        public override bool Equals(object? obj)
        {
            ObjectEqualsCalled = true;
            return base.Equals(obj);
        }

        public override bool Equals(RecordQueue<int>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion
}

