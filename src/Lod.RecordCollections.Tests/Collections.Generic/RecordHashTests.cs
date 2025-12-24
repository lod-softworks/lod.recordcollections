namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordSetTests
{
    [TestInitialize]
    public void SetUp()
    {
        RecordCollectionComparer.Default = new RecordCollectionComparer();
    }

    /// <remarks>This test is a sanity check to ensure that the default HashSet.Equals behavior is as expected.</remarks>
    [TestMethod]
    public void Set_SameInts_NotEqualsMatchingSet()
    {
        // Arrange
        HashSet<int> set1 = [92, 117, 420,];
        HashSet<int> set2 = [92, 117, 420,];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void RecordSet_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
        RecordCollectionComparer.Default = comparer;

        // Act
        RecordSet<int> set = [];

        // Assert
        Assert.AreSame(comparer, set.Comparer);
    }

    [TestMethod]
    public void RecordSet_CustomComparerConstructor_UsesProvidedComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();

        // Act
        RecordSet<int> set = new(comparer);

        // Assert
        Assert.AreSame(comparer, set.Comparer);
    }

    [TestMethod]
    public void RecordSet_Operators_UseTypedEquals()
    {
        OperatorAwareRecordSet left = new([92, 117]);
        OperatorAwareRecordSet right = new([92, 117]);

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
    public void RecordSet_SameInts_EqualsMatchingSet()
    {
        // Arrange
        RecordSet<int> set1 = [92, 117, 420,];
        RecordSet<int> set2 = [92, 117, 420,];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameStrings_EqualsMatchingSet()
    {
        // Arrange
        RecordSet<string> set1 = ["92", "117", "420",];
        RecordSet<string> set2 = ["92", "117", "420",];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameRecords_EqualsMatchingSet()
    {
        // Arrange
        RecordSet<Number> set1 = [new Number(92), new Number(117), new Number(420),];
        RecordSet<Number> set2 = [new Number(92), new Number(117), new Number(420),];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameInts_DifferentOrder_EqualsSimilarSet()
    {
        // Arrange
        RecordSet<int> set1 = [92, 117, 420,];
        RecordSet<int> set2 = [117, 420, 92,];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    //[TestMethod]
    //public void RecordSet_ClonedRecord_NewUnderlyingElements()
    //{
    //    // Arrange
    //    RecordSet<Number> set1 = new() { new Number(92), new Number(117), new Number(420), };

    //    // Act
    //    RecordSet<Number> set2 = (RecordSet<Number>)typeof(RecordSet<Number>).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { set1, });

    //    // Assert
    //    foreach (Number number in set1)
    //    {
    //        Assert.IsFalse(!set2.Contains(number), $"Reference of items are equivielent.");
    //    }
    //}

    [TestMethod]
    public void RecordSet_DeserializedNewtonsoft_EqualsReserialized()
    {
        // Arrange
        RecordSet<Number> set = [new Number(92), new Number(117), new Number(420),];

        // Act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(set);
        RecordSet<Number>? recordSet = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordSet<Number>>(json);
        HashSet<Number>? hashSet = Newtonsoft.Json.JsonConvert.DeserializeObject<HashSet<Number>>(json);

        // Assert
        Assert.IsNotNull(recordSet, "Deserialized record set is null.");
        Assert.IsNotNull(hashSet, "Deserialized hash set is null.");
        Assert.IsTrue(set.Equals(recordSet), "Deserialized set is not equal to the original set.");
        Assert.IsTrue(set.IsSupersetOf(hashSet), "Deserialized set is not a subset of the original set.");
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordSet_DeserializedSystemTextJson_EqualsReserialized()
    {
        // Arrange
        RecordSet<Number> set = [new Number(92), new Number(117), new Number(420),];

        // Act
        string json = System.Text.Json.JsonSerializer.Serialize(set);
        RecordSet<Number>? recordSet = System.Text.Json.JsonSerializer.Deserialize<RecordSet<Number>>(json);
        HashSet<Number>? hashSet = System.Text.Json.JsonSerializer.Deserialize<HashSet<Number>>(json);

        // Assert
        Assert.IsNotNull(recordSet, "Deserialized record set is null.");
        Assert.IsNotNull(hashSet, "Deserialized hash set is null.");
        Assert.IsTrue(set.Equals(recordSet), "Deserialized set is not equal to the original set.");
        Assert.IsTrue(set.IsSupersetOf(hashSet), "Deserialized set is not a subset of the original set.");
    }
#endif

    #region Support Types

    private sealed class OperatorAwareRecordSet(IEnumerable<int> values) : RecordSet<int>(values)
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

        public override bool Equals(RecordSet<int>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion
}
