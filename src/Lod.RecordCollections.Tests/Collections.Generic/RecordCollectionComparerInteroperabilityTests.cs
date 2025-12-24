using System.Collections.Generic;

namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public sealed class RecordCollectionComparerInteroperabilityTests
{
    [TestInitialize]
    public void SetUp()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = new RecordCollectionComparer();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [TestMethod]
    public void RecordList_Equals_Matching_SystemList()
    {
        RecordList<int> record = [1, 2, 3];
        List<int> system = [1, 2, 3];

        Assert.IsTrue(record.Equals(system));
    }

    [TestMethod]
    public void RecordQueue_Equals_Matching_SystemQueue()
    {
        RecordQueue<int> record = new([1, 2, 3]);
        Queue<int> system = new([1, 2, 3]);

        Assert.IsTrue(record.Equals(system));
    }

    [TestMethod]
    public void RecordStack_Equals_Matching_SystemStack()
    {
        RecordStack<int> record = new([1, 2, 3]);
        Stack<int> system = new([1, 2, 3]);

        Assert.IsTrue(record.Equals(system));
    }

    [TestMethod]
    public void RecordSet_Equals_Matching_SystemHashSet()
    {
        RecordSet<int> record = new([1, 2, 3]);
        HashSet<int> system = new([3, 2, 1]);

        Assert.IsTrue(record.Equals(system));
    }

    [TestMethod]
    public void RecordDictionary_Equals_Matching_SystemDictionary()
    {
        RecordDictionary<int, string> record = new() { [1] = "1", [2] = "2", [3] = "3" };
        Dictionary<int, string> system = new() { [3] = "3", [2] = "2", [1] = "1" };

        Assert.IsTrue(record.Equals(system));
    }
}

