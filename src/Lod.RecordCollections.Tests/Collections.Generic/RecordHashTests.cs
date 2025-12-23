using System.Reflection;

namespace System.Collections.Tests.Generic;

[TestClass]
public class RecordSetTests
{
    // sanity check test
    [TestMethod]
    public void Set_SameInts_NotEqualsMatchingSet()
    {
        // arrange
        HashSet<int> set1 = [92, 117, 420,];
        HashSet<int> set2 = [92, 117, 420,];

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordSet_DefaultConstructor_UsesDefaultComparer()
    {
        // arrange
        global::System.Collections.IRecordCollectionComparer original = RecordCollectionComparer.Default;
        TestRecordCollectionComparer overrideComparer = new();
        RecordCollectionComparer.Default = overrideComparer;

        try
        {
            // act
            RecordSet<int> set = [];

            // assert
            Assert.AreSame(overrideComparer, set.Comparer);
        }
        finally
        {
            RecordCollectionComparer.Default = original;
        }
    }

    [TestMethod]
    public void RecordSet_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        RecordSet<int> set = new(comparer);

        // assert
        Assert.AreSame(comparer, set.Comparer);
    }

    [TestMethod]
    public void RecordSet_SameInts_EqualsMatchingSet()
    {
        // arrange
        RecordSet<int> set1 = [92, 117, 420,];
        RecordSet<int> set2 = [92, 117, 420,];

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameStrings_EqualsMatchingSet()
    {
        // arrange
        RecordSet<string> set1 = ["92", "117", "420",];
        RecordSet<string> set2 = ["92", "117", "420",];

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameRecords_EqualsMatchingSet()
    {
        // arrange
        RecordSet<Number> set1 = [new Number(92), new Number(117), new Number(420),];
        RecordSet<Number> set2 = [new Number(92), new Number(117), new Number(420),];

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordSet_SameInts_DifferentOrder_EqualsSimilarSet()
    {
        // arrange
        RecordSet<int> set1 = [92, 117, 420,];
        RecordSet<int> set2 = [117, 420, 92,];

        // act
        bool areEqual = set1.Equals(set2);

        // assert
        Assert.IsTrue(areEqual);
    }

    //[TestMethod]
    //public void RecordSet_ClonedRecord_NewUnderlyingElements()
    //{
    //    // arrange
    //    RecordSet<Number> set1 = new() { new Number(92), new Number(117), new Number(420), };

    //    // act
    //    RecordSet<Number> set2 = (RecordSet<Number>)typeof(RecordSet<Number>).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { set1, });

    //    // assert
    //    foreach (Number number in set1)
    //    {
    //        Assert.IsFalse(!set2.Contains(number), $"Reference of items are equivielent.");
    //    }
    //}

    [TestMethod]
    public void RecordSet_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordSet<Number> set = [new Number(92), new Number(117), new Number(420),];

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

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordSet_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordSet<Number> set = [new Number(92), new Number(117), new Number(420),];

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
#endif

    #region Support Types

    sealed record Number
    {
        public int Value { get; set; }

        public Number(int value)
        {
            Value = value;
        }
    }

    #endregion
}
