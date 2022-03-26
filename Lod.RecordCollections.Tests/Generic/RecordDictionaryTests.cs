namespace System.Collections.Tests.Generic;

[TestClass]
public class RecordDictionaryTests
{
    // sanity check test
    [TestMethod]
    public void List_SameInts_NotEqualsMatchingList()
    {
        // arrange
        List<int> list1 = new() { 92, 117, 420, };
        List<int> list2 = new() { 92, 117, 420, };

        // act
        bool areEqual = list1.Equals(list2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordList_SameInts_EqualsMatchingList()
    {
        // arrange
        RecordList<int> list1 = new() { 92, 117, 420, };
        RecordList<int> list2 = new() { 92, 117, 420, };

        // act
        bool areEqual = list1.Equals(list2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameStrings_EqualsMatchingList()
    {
        // arrange
        RecordList<string> list1 = new() { "92", "117", "420", };
        RecordList<string> list2 = new() { "92", "117", "420", };

        // act
        bool areEqual = list1.Equals(list2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameRecords_EqualsMatchingList()
    {
        // arrange
        RecordList<Number> list1 = new() { new Number(92), new Number(117), new Number(420), };
        RecordList<Number> list2 = new() { new Number(92), new Number(117), new Number(420), };

        // act
        bool areEqual = list1.Equals(list2);

        // assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void RecordList_SameInts_DifferentOrder_NotEqualsSimilarList()
    {
        // arrange
        RecordList<int> list1 = new() { 92, 117, 420, };
        RecordList<int> list2 = new() { 117, 420, 92, };

        // act
        bool areEqual = list1.Equals(list2);

        // assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordList_DeserializedNewtonsoft_EqualsReserialized()
    {
        // arrange
        RecordList<Number> list = new() { new Number(92), new Number(117), new Number(420), };

        // act
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        RecordList<Number>? recordList = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordList<Number>>(json);
        Number[]? systemList = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // assert
        Assert.IsNotNull(recordList, "Deserialized record list is null.");
        Assert.IsNotNull(systemList, "Deserialized list is null.");
        Assert.IsTrue(list.Equals(recordList), "Deserialized list is not equal to the original list.");
        for (int i = 0; i < list.Count; i++)
        {
            Assert.IsTrue(list[i] == recordList[i], "Deserialized list is not a subset of the original list.");
        }
    }

    [TestMethod]
    public void RecordList_DeserializedSystemTextJson_EqualsReserialized()
    {
        // arrange
        RecordList<Number> list = new() { new Number(92), new Number(117), new Number(420), };

        // act
        string json = System.Text.Json.JsonSerializer.Serialize(list);
        RecordList<Number>? recordList = System.Text.Json.JsonSerializer.Deserialize<RecordList<Number>>(json);
        Number[]? systemList = Newtonsoft.Json.JsonConvert.DeserializeObject<Number[]>(json);

        // assert
        Assert.IsNotNull(recordList, "Deserialized record list is null.");
        Assert.IsNotNull(systemList, "Deserialized list is null.");
        Assert.IsTrue(list.Equals(recordList), "Deserialized list is not equal to the original list.");
        for (int i = 0; i < list.Count; i++)
        {
            Assert.IsTrue(list[i] == recordList[i], "Deserialized list is not a subset of the original list.");
        }
    }

    #region Support Types

    sealed record Number(int Value);

    #endregion
}
