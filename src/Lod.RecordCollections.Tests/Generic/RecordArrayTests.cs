namespace System.Collections.Tests.Generic;

[TestClass]
public class RecordArrayTests
{
    [TestInitialize]
    public void Setup() => Assert.Inconclusive("RecordArray is not ready to test.");

    // sanity check test
    [TestMethod]
    public void Array_SameInts_NotEqualsMatchingArray()
    {
        // arrange
        int[] array1 = new[] { 92, 117, 420, };
        int[] array2 = new[] { 92, 117, 420, };

        // act
        bool areEqual = array1.Equals(array2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordArray_SameInts_EqualsMatchingArray()
    {
        // arrange
        RecordArray<int> array1 = new[] { 92, 117, 420, };
        RecordArray<int> array2 = new[] { 92, 117, 420, };

        // act
        bool areEqual = array1.Equals(array2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordArray_SameStrings_EqualsMatchingArray()
    {
        // arrange
        RecordArray<string> array1 = new[] { "92", "117", "420", };
        RecordArray<string> array2 = new[] { "92", "117", "420", };

        // act
        bool areEqual = array1.Equals(array2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordArray_SameRecords_EqualsMatchingArray()
    {
        // arrange
        RecordArray<Number> array1 = new[] { new Number(92), new Number(117), new Number(420), };
        RecordArray<Number> array2 = new[] { new Number(92), new Number(117), new Number(420), };

        // act
        bool areEqual = array1.Equals(array2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordArray_SameInts_DifferentOrder_NotEqualsSimilarArray()
    {
        // arrange
        RecordArray<int> array1 = new[] { 92, 117, 420, };
        RecordArray<int> array2 = new[] { 117, 420, 92, };

        // act
        bool areEqual = array1.Equals(array2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordArray_ClonedRecord_NewUnderlyingElements()
    {
        // arrange
        RecordArray<Number> array1 = new() { new Number(92), new Number(117), new Number(420), };

        // act
        RecordArray<Number> array2 = array1 with { };

        // assert
        for (int i = 0; i < array1.Count; i++)
        {
            Assert.IsFalse(ReferenceEquals(array1[i], array2[i]), $"Reference of item {i} are equivielent.");
        }
    }

    [TestMethod]
    public void RecordArray_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordArray<Number> array = new[] { new Number(92), new Number(117), new Number(420), };

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(array);
        RecordArray<Number>? recordArray = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordArray<Number>>(json);
        Number[]? systemArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // assert
        Assert.IsNotNull(recordArray, "Deserialized record array is null.");
        Assert.IsNotNull(systemArray, "Deserialized array is null.");
        Assert.IsTrue(array.Equals(recordArray), "Deserialized array is not equal to the original array.");
        for (int i = 0; i < array.Length; i++)
        {
            Assert.IsTrue(array[i] == recordArray[i], "Deserialized array is not a subset of the original array.");
        }
    }

    [TestMethod]
    public void RecordArray_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordArray<Number> array = new() { new Number(92), new Number(117), new Number(420), };

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(array);
        RecordArray<Number>? recordArray = System.Text.Json.JsonSerializer.Deserialize<RecordArray<Number>>(json);
        Number[]? systemArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // assert
        Assert.IsNotNull(recordArray, "Deserialized record array is null.");
        Assert.IsNotNull(systemArray, "Deserialized array is null.");
        Assert.IsTrue(array.Equals(recordArray), "Deserialized array is not equal to the original array.");
        for (int i = 0; i < array.Length; i++)
        {
            Assert.IsTrue(array[i] == recordArray[i], "Deserialized array is not a subset of the original array.");
        }
    }

    #region Support Types

    sealed record Number(int Value);

    #endregion
}
