namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordQueueTests
{
    [TestInitialize]
    public void SetUp()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = new RecordCollectionComparer();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    // sanity check test
    [TestMethod]
    public void Queue_SameInts_NotEqualsMatchingQueue()
    {
        // arrange
        Queue<int> queue1 = new([92, 117, 420]);
        Queue<int> queue2 = new([92, 117, 420]);

        // act
        bool areEqual = queue1.Equals(queue2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void RecordQueue_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = comparer;
#pragma warning restore CS0618 // Type or member is obsolete

        // Act
        RecordQueue<int> queue = new();

        // Assert
        Assert.AreSame(comparer, queue.Comparer);
    }

    [TestMethod]
    public void RecordQueue_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        RecordQueue<int> queue = new(comparer);

        // assert
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
        // arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([92, 117, 420]);

        // act
        bool areEqual = queue1.Equals(queue2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_SameStrings_EqualsMatchingQueue()
    {
        // arrange
        RecordQueue<string> queue1 = new(["92", "117", "420"]);
        RecordQueue<string> queue2 = new(["92", "117", "420"]);

        // act
        bool areEqual = queue1.Equals(queue2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_SameRecords_EqualsMatchingQueue()
    {
        // arrange
        RecordQueue<Number> queue1 = new([new Number(92), new Number(117), new Number(420)]);
        RecordQueue<Number> queue2 = new([new Number(92), new Number(117), new Number(420)]);

        // act
        bool areEqual = queue1.Equals(queue2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordQueue_SameInts_DifferentOrder_NotEqualsSimilarQueue()
    {
        // arrange
        RecordQueue<int> queue1 = new([92, 117, 420]);
        RecordQueue<int> queue2 = new([420, 117, 92]);

        // act
        bool areEqual = queue1.Equals(queue2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordQueue_EnqueueDequeue_OrderPreserved()
    {
        // arrange
        RecordQueue<int> queue = new();
        queue.Enqueue(92);
        queue.Enqueue(117);
        queue.Enqueue(420);

        // act
        int first = queue.Dequeue();
        int second = queue.Dequeue();
        int third = queue.Dequeue();

        // assert
        Assert.AreEqual(92, first);
        Assert.AreEqual(117, second);
        Assert.AreEqual(420, third);
    }

    [TestMethod]
    public void RecordQueue_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordQueue<Number> queue = new([new Number(92), new Number(117), new Number(420)]);

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(queue);
        RecordQueue<Number>? recordQueue = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordQueue<Number>>(json);
        Queue<Number>? systemQueue = Newtonsoft.Json.JsonConvert.DeserializeObject<Queue<Number>>(json);

        // assert
        Assert.IsNotNull(recordQueue, "Deserialized record queue is null.");
        Assert.IsNotNull(systemQueue, "Deserialized queue is null.");
        Assert.IsTrue(queue.Equals(recordQueue), "Deserialized queue is not equal to the original queue.");
        Assert.AreEqual(queue.Count, recordQueue.Count, "Deserialized queue count does not match.");
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordQueue_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordQueue<Number> queue = new([new Number(92), new Number(117), new Number(420)]);

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(queue);
        RecordQueue<Number>? recordQueue = System.Text.Json.JsonSerializer.Deserialize<RecordQueue<Number>>(json);
        Queue<Number>? systemQueue = System.Text.Json.JsonSerializer.Deserialize<Queue<Number>>(json);

        // assert
        Assert.IsNotNull(recordQueue, "Deserialized record queue is null.");
        Assert.IsNotNull(systemQueue, "Deserialized queue is null.");
        Assert.IsTrue(queue.Equals(recordQueue), "Deserialized queue is not equal to the original queue.");
        Assert.AreEqual(queue.Count, recordQueue.Count, "Deserialized queue count does not match.");
    }
#endif

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

