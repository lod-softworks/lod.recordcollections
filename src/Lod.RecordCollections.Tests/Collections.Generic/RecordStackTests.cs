namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordStackTests
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
    public void Stack_SameInts_NotEqualsMatchingStack()
    {
        // arrange
        Stack<int> stack1 = new([92, 117, 420]);
        Stack<int> stack2 = new([92, 117, 420]);

        // act
        bool areEqual = stack1.Equals(stack2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void RecordStack_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = comparer;
#pragma warning restore CS0618 // Type or member is obsolete

        // Act
        RecordStack<int> stack = new();

        // Assert
        Assert.AreSame(comparer, stack.Comparer);
    }

    [TestMethod]
    public void RecordStack_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        RecordStack<int> stack = new(comparer);

        // assert
        Assert.AreSame(comparer, stack.Comparer);
    }

    [TestMethod]
    public void RecordStack_Operators_UseTypedEquals()
    {
        OperatorAwareRecordStack left = new([92, 117]);
        OperatorAwareRecordStack right = new([92, 117]);

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
    public void RecordStack_SameInts_EqualsMatchingStack()
    {
        // arrange
        RecordStack<int> stack1 = new([92, 117, 420]);
        RecordStack<int> stack2 = new([92, 117, 420]);

        // act
        bool areEqual = stack1.Equals(stack2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordStack_SameStrings_EqualsMatchingStack()
    {
        // arrange
        RecordStack<string> stack1 = new(["92", "117", "420"]);
        RecordStack<string> stack2 = new(["92", "117", "420"]);

        // act
        bool areEqual = stack1.Equals(stack2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordStack_SameRecords_EqualsMatchingStack()
    {
        // arrange
        RecordStack<Number> stack1 = new([new Number(92), new Number(117), new Number(420)]);
        RecordStack<Number> stack2 = new([new Number(92), new Number(117), new Number(420)]);

        // act
        bool areEqual = stack1.Equals(stack2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordStack_SameInts_DifferentOrder_NotEqualsSimilarStack()
    {
        // arrange
        RecordStack<int> stack1 = new([92, 117, 420]);
        RecordStack<int> stack2 = new([420, 117, 92]);

        // act
        bool areEqual = stack1.Equals(stack2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordStack_PushPop_OrderPreserved()
    {
        // arrange
        RecordStack<int> stack = new();
        stack.Push(92);
        stack.Push(117);
        stack.Push(420);

        // act
        int third = stack.Pop();
        int second = stack.Pop();
        int first = stack.Pop();

        // assert
        Assert.AreEqual(420, third);
        Assert.AreEqual(117, second);
        Assert.AreEqual(92, first);
    }

    [TestMethod]
    public void RecordStack_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordStack<Number> stack = new([new Number(92), new Number(117), new Number(420)]);

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(stack);
        RecordStack<Number>? recordStack = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordStack<Number>>(json);
        Stack<Number>? systemStack = Newtonsoft.Json.JsonConvert.DeserializeObject<Stack<Number>>(json);

        // assert
        Assert.IsNotNull(recordStack, "Deserialized record stack is null.");
        Assert.IsNotNull(systemStack, "Deserialized stack is null.");
        Assert.IsTrue(stack.Equals(recordStack), "Deserialized stack is not equal to the original stack.");
        Assert.AreEqual(stack.Count, recordStack.Count, "Deserialized stack count does not match.");
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordStack_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordStack<Number> stack = new([new Number(92), new Number(117), new Number(420)]);

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(stack);
        RecordStack<Number>? recordStack = System.Text.Json.JsonSerializer.Deserialize<RecordStack<Number>>(json);
        Stack<Number>? systemStack = System.Text.Json.JsonSerializer.Deserialize<Stack<Number>>(json);

        // assert
        Assert.IsNotNull(recordStack, "Deserialized record stack is null.");
        Assert.IsNotNull(systemStack, "Deserialized stack is null.");
        Assert.IsTrue(stack.Equals(recordStack), "Deserialized stack is not equal to the original stack.");
        Assert.AreEqual(stack.Count, recordStack.Count, "Deserialized stack count does not match.");
    }
#endif

    #region Support Types

    private sealed class OperatorAwareRecordStack(IEnumerable<int> values) : RecordStack<int>(values)
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

        public override bool Equals(RecordStack<int>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion
}

