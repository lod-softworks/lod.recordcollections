using System.Reflection;

namespace System.Collections.Tests.Generic;

[TestClass]
public class RecordDictionaryTests
{
    // sanity check test
    [TestMethod]
    public void Dictionary_SameInts_NotEqualsMatchingDictionary()
    {
        // arrange
        Dictionary<int, string> dictionary1 = new() { { 92, "92" }, { 117, "117" }, { 420, "420" }, };
        Dictionary<int, string> dictionary2 = new() { { 92, "92" }, { 117, "117" }, { 420, "420" }, };

        // act
        bool areEqual = dictionary1.Equals(dictionary2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void RecordDictionary_DefaultConstructor_UsesDefaultComparer()
    {
        TestRecordCollectionComparer overrideComparer = new();
        using (ComparerTestUtilities.OverrideDefaultComparer(overrideComparer))
        {
            RecordDictionary<int, string> dictionary = new();
            Assert.AreSame(overrideComparer, dictionary.Comparer);
        }
    }

    [TestMethod]
    public void RecordDictionary_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        RecordDictionary<int, string> dictionary = new(comparer);

        // assert
        Assert.AreSame(comparer, dictionary.Comparer);
    }

    [TestMethod]
    public void RecordDictionary_Operators_UseTypedEquals()
    {
        OperatorAwareRecordDictionary left = OperatorAwareRecordDictionary.Create();
        OperatorAwareRecordDictionary right = OperatorAwareRecordDictionary.Create();

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
    public void RecordDictionary_SameInts_EqualsMatchingDictionary()
    {
        // arrange
        RecordDictionary<int, string> dictionary1 = new() { { 92, "92" }, { 117, "117" }, { 420, "420" }, };
        RecordDictionary<int, string> dictionary2 = new() { { 92, "92" }, { 117, "117" }, { 420, "420" }, };

        // act
        bool areEqual = dictionary1.Equals(dictionary2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordDictionary_SameStrings_EqualsMatchingDictionary()
    {
        // arrange
        RecordDictionary<string, int> dictionary1 = new() { { "92", 92 }, { "117", 117 }, { "420", 420 }, };
        RecordDictionary<string, int> dictionary2 = new() { { "92", 92 }, { "117", 117 }, { "420", 420 }, };

        // act
        bool areEqual = dictionary1.Equals(dictionary2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordDictionary_SameRecords_EqualsMatchingDictionary()
    {
        // arrange
        RecordDictionary<string, Number> dictionary1 = new() { { "1", new Number(92) }, { "2", new Number(117) }, { "3", new Number(420) }, };
        RecordDictionary<string, Number> dictionary2 = new() { { "1", new Number(92) }, { "2", new Number(117) }, { "3", new Number(420) }, };

        // act
        bool areEqual = dictionary1.Equals(dictionary2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordDictionary_SameInts_DifferentOrder_NotEqualsSimilarDictionary()
    {
        // arrange
        Dictionary<int, string> dictionary1 = new() { { 92, "92" }, { 117, "117" }, { 420, "420" }, };
        Dictionary<int, string> dictionary2 = new() { { 117, "117" }, { 420, "420" }, { 92, "92" }, };

        // act
        bool areEqual = dictionary1.Equals(dictionary2);

        // assert
        Assert.IsFalse(areEqual);
    }

    //[TestMethod]
    //public void RecordDictionary_ClonedRecords_NewUnderlyingElements()
    //{
    //    // arrange
    //    RecordDictionary<Number> dictionary1 = new() { new Number(92), new Number(117), new Number(420), };

    //    // act
    //    RecordDictionary<Number> dictionary2 = (RecordDictionary<Number>)typeof(RecordDictionary<Number>).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { dictionary1, });

    //    // assert
    //    for (int i = 0; i < dictionary1.Count; i++)
    //    {
    //        Assert.IsFalse(ReferenceEquals(dictionary1[i], dictionary2[i]), $"Reference of item {i} are equivielent.");
    //    }
    //}

    [TestMethod]
    public void RecordDictionary_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordDictionary<string, Number> dictionary = new() { { "1", new Number(92) }, { "2", new Number(117) }, { "3", new Number(420) }, };

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
        RecordDictionary<string, Number>? recordDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordDictionary<string, Number>>(json);
        Dictionary<string, Number>? systemDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Number>>(json);

        // assert
        Assert.IsNotNull(recordDictionary, "Deserialized record dictionary is null.");
        Assert.IsNotNull(systemDictionary, "Deserialized dictionary is null.");
        Assert.IsTrue(dictionary.Equals(recordDictionary), "Deserialized dictionary is not equal to the original dictionary.");
        foreach (KeyValuePair<string, Number> kv in dictionary)
        {
            Assert.IsTrue(dictionary[kv.Key] == recordDictionary[kv.Key], "Deserialized dictionary is not a subset of the original dictionary.");
        }
    }

#if !NETFRAMEWORK
    [TestMethod]
    public void RecordDictionary_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordDictionary<string, Number> dictionary = new() { { "1", new Number(92) }, { "2", new Number(117) }, { "3", new Number(420) }, };

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(dictionary);
        RecordDictionary<string, Number>? recordDictionary = System.Text.Json.JsonSerializer.Deserialize<RecordDictionary<string, Number>>(json);
        Dictionary<string, Number>? systemDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Number>>(json);

        // assert
        Assert.IsNotNull(recordDictionary, "Deserialized record dictionary is null.");
        Assert.IsNotNull(systemDictionary, "Deserialized dictionary is null.");
        Assert.IsTrue(dictionary.Equals(recordDictionary), "Deserialized dictionary is not equal to the original dictionary.");
        foreach (KeyValuePair<string, Number> kv in dictionary)
        {
            Assert.IsTrue(dictionary[kv.Key] == recordDictionary[kv.Key], "Deserialized dictionary is not a subset of the original dictionary.");
        }
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

    private sealed class OperatorAwareRecordDictionary : RecordDictionary<int, string>
    {
        private OperatorAwareRecordDictionary(Dictionary<int, string> values) : base(values) { }

        public bool TypedEqualsCalled { get; private set; }
        public bool ObjectEqualsCalled { get; private set; }

        public static OperatorAwareRecordDictionary Create() =>
            new(new Dictionary<int, string> { { 1, "1" }, { 2, "2" } });

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

        public override bool Equals(RecordDictionary<int, string>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion
}
