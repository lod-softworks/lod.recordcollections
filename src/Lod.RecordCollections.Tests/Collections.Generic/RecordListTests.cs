namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordListTests
{
    [TestInitialize]
    public void SetUp()
    {
        RecordCollectionComparer.Default = new RecordCollectionComparer();
    }

    /// <remarks>This test is a sanity check to ensure that the default List.Equals behavior is as expected.</remarks>
    [TestMethod]
    public void List_SameInts_NotEqualsMatchingList()
    {
        // Arrange
        List<int> list1 = [92, 117, 420,];
        List<int> list2 = [92, 117, 420,];

        // Act
        bool areEqual = list1.Equals(list2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void RecordList_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
        RecordCollectionComparer.Default = comparer;

        // Act
        RecordList<int> list = [];

        // Assert
        Assert.AreSame(comparer, list.Comparer);
    }

    [TestMethod]
    public void RecordList_CustomComparerConstructor_UsesProvidedComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();

        // Act
        RecordList<int> list = new(comparer);

        // Assert
        Assert.AreSame(comparer, list.Comparer);
    }

    [TestMethod]
    public void RecordList_Operators_UseTypedEquals()
    {
        OperatorAwareRecordList left = new([92, 117]);
        OperatorAwareRecordList right = new([92, 117]);

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
    public void RecordList_SameInts_EqualsMatchingList()
    {
        // Arrange
        RecordList<int> list1 = [92, 117, 420,];
        RecordList<int> list2 = [92, 117, 420,];

        // Act
        bool areEqual = list1.Equals(list2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameStrings_EqualsMatchingList()
    {
        // Arrange
        RecordList<string> list1 = ["92", "117", "420",];
        RecordList<string> list2 = ["92", "117", "420",];

        // Act
        bool areEqual = list1.Equals(list2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameRecords_EqualsMatchingList()
    {
        // Arrange
        RecordList<Number> list1 = [new Number(92), new Number(117), new Number(420),];
        RecordList<Number> list2 = [new Number(92), new Number(117), new Number(420),];

        // Act
        bool areEqual = list1.Equals(list2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameInts_DifferentOrder_NotEqualsSimilarList()
    {
        // Arrange
        RecordList<int> list1 = [92, 117, 420,];
        RecordList<int> list2 = [117, 420, 92,];

        // Act
        bool areEqual = list1.Equals(list2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    //[TestMethod]
    //public void RecordList_ClonedRecord_NewUnderlyingElements()
    //{
    //    // Arrange
    //    RecordList<Number> list1 = new() { new Number(92), new Number(117), new Number(420), };

    //    // Act
    //    RecordList<Number> list2 = (RecordList<Number>)typeof(RecordList<Number>).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { list1, });

    //    // Assert
    //    for (int i = 0; i < list1.Count; i++)
    //    {
    //        Assert.IsFalse(ReferenceEquals(list1[i], list2[i]), $"Reference of item {i} are equivielent.");
    //    }
    //}

    [TestMethod]
    public void RecordList_DeserializedNewtonsoft_EqualsReserialized()
    {
        // Arrange
        RecordList<Number> list = [new Number(92), new Number(117), new Number(420),];

        // Act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        RecordList<Number>? recordList = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordList<Number>>(json);
        Number[]? systemList = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // Assert
        Assert.IsNotNull(recordList, "Deserialized record list is null.");
        Assert.IsNotNull(systemList, "Deserialized list is null.");
        Assert.IsTrue(list.Equals(recordList), "Deserialized list is not equal to the original list.");
        for (int i = 0; i < list.Count; i++)
        {
            Assert.IsTrue(list[i] == recordList[i], "Deserialized list is not a subset of the original list.");
        }
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordList_DeserializedSystemTextJson_EqualsReserialized()
    {
        // Arrange
        RecordList<Number> list = [new Number(92), new Number(117), new Number(420),];

        // Act
        string json = System.Text.Json.JsonSerializer.Serialize(list);
        RecordList<Number>? recordList = System.Text.Json.JsonSerializer.Deserialize<RecordList<Number>>(json);
        Number[]? systemList = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // Assert
        Assert.IsNotNull(recordList, "Deserialized record list is null.");
        Assert.IsNotNull(systemList, "Deserialized list is null.");
        Assert.IsTrue(list.Equals(recordList), "Deserialized list is not equal to the original list.");
        for (int i = 0; i < list.Count; i++)
        {
            Assert.IsTrue(list[i] == recordList[i], "Deserialized list is not a subset of the original list.");
        }
    }
#endif

    #region Support Types

    private sealed class OperatorAwareRecordList(IEnumerable<int> values) : RecordList<int>(values)
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

        public override bool Equals(RecordList<int>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion
}
