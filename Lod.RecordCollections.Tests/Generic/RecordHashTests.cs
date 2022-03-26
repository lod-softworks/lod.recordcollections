namespace System.Collections.Tests.Generic;

[TestClass]
public class RecordSetTests
{
    // sanity check test
    [TestMethod]
    public void Set_SameInts_NotEqualsMatchingSet()
    {
        // arrange
        HashSet<int> set1 = new() { 92, 117, 420, };
        HashSet<int> set2 = new() { 92, 117, 420, };

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameInts_EqualsMatchingSet()
    {
        // arrange
        RecordSet<int> set1 = new() { 92, 117, 420, };
        RecordSet<int> set2 = new() { 92, 117, 420, };

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameStrings_EqualsMatchingSet()
    {
        // arrange
        RecordSet<string> set1 = new() { "92", "117", "420", };
        RecordSet<string> set2 = new() { "92", "117", "420", };

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameRecords_EqualsMatchingSet()
    {
        // arrange
        RecordSet<Number> set1 = new() { new Number(92), new Number(117), new Number(420), };
        RecordSet<Number> set2 = new() { new Number(92), new Number(117), new Number(420), };

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameInts_DifferentOrder_EqualsSimilarSet()
    {
        // arrange
        RecordSet<int> set1 = new() { 92, 117, 420, };
        RecordSet<int> set2 = new() { 117, 420, 92, };

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordSet<Number> set = new() { new Number(92), new Number(117), new Number(420), };

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(set);
        RecordSet<Number>? recordSet = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordSet<Number>>(json);
        HashSet<Number>? hashSet = Newtonsoft.Json.JsonConvert.DeserializeObject<HashSet<Number>>(json);

        // assert
        Assert.IsNotNull(recordSet, "Deserialized record set is null.");
        Assert.IsNotNull(hashSet, "Deserialized hash set is null.");
        Assert.IsTrue(set.Equals(recordSet), "Deserialized set is not equal to the original set.");
        Assert.IsTrue(set.IsSupersetOf(hashSet), "Deserialized set is not a subset of the original set.");
    }

    [TestMethod]
    public void RecordSet_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordSet<Number> set = new() { new Number(92), new Number(117), new Number(420), };

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(set);
        RecordSet<Number>? recordSet = System.Text.Json.JsonSerializer.Deserialize<RecordSet<Number>>(json);
        HashSet<Number>? hashSet = System.Text.Json.JsonSerializer.Deserialize<HashSet<Number>>(json);

        // assert
        Assert.IsNotNull(recordSet, "Deserialized record set is null.");
        Assert.IsNotNull(hashSet, "Deserialized hash set is null.");
        Assert.IsTrue(set.Equals(recordSet), "Deserialized set is not equal to the original set.");
        Assert.IsTrue(set.IsSupersetOf(hashSet), "Deserialized set is not a subset of the original set.");
    }

    #region Support Types

    sealed record Number(int Value);

    #endregion
}
