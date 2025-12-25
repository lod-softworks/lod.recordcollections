using System.Diagnostics;

namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class RecordSetTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void SetUp()
    {
        RecordCollectionComparer.Default = new RecordCollectionComparer();
    }

    private static int GetSizeOrDefault(int @default)
    {
        string? raw = Environment.GetEnvironmentVariable("RECORDCOLLECTION_STRESS_SIZE");
        return int.TryParse(raw, out int size) && size > 0 ? size : @default;
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
    [DoNotParallelize]
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
    public void RecordSet_Equals_Matching_SystemHashSet()
    {
        // Arrange
        RecordSet<int> record = new([1, 2, 3]);
        HashSet<int> system = new([3, 2, 1]);

        // Act & Assert
        Assert.IsTrue(record.Equals(system));
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

    [TestMethod]
    public void RecordSet_DifferentValues_NotEquals()
    {
        // Arrange
        RecordSet<int> set1 = [92, 117, 420];
        RecordSet<int> set2 = [92, 117, 421];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordSet_DifferentSizes_NotEquals()
    {
        // Arrange
        RecordSet<int> set1 = [92, 117, 420];
        RecordSet<int> set2 = [92, 117];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordSet_EmptyVsNonEmpty_NotEquals()
    {
        // Arrange
        RecordSet<int> set1 = [];
        RecordSet<int> set2 = [92, 117, 420];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void RecordSet_OneValueDifferent_NotEquals()
    {
        // Arrange
        RecordSet<int> set1 = [1, 2, 3, 4, 5];
        RecordSet<int> set2 = [1, 2, 99, 4, 5];

        // Act
        bool areEqual = set1.Equals(set2);

        // Assert
        Assert.IsFalse(areEqual);
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

    [TestMethod]
    public void Stress_RecordSet_Int32_Compare_And_Time()
    {
        int n = GetSizeOrDefault(@default: 1_000_000);

        RecordSet<int> left = new(capacity: n);
        RecordSet<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            left.Add(i);
            right.Add(i);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordSet<int>.Equals (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");

        sw.Restart();
        _ = left.GetHashCode();
        _ = right.GetHashCode();
        sw.Stop();
        TestContext.WriteLine($"RecordSet<int>.GetHashCode x2 (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    [RepeatTestMethod(10)]
    public void Stress_RecordSet_Int32_Random_Compare_And_Time()
    {
        int n = GetSizeOrDefault(@default: 1_000_000);
        Random random = new();

        RecordSet<int> left = new(capacity: n);
        RecordSet<int> right = new(capacity: n);

        for (int i = 0; i < n; i++)
        {
            int value = random.Next();
            left.Add(value);
            right.Add(value);
        }

        Stopwatch sw = Stopwatch.StartNew();
        Assert.IsTrue(left.Equals(right));
        sw.Stop();
        TestContext.WriteLine($"RecordSet<int>.Equals with random values (n={n:n0}) = {sw.ElapsedMilliseconds:n0} ms");
    }

    [TestMethod]
    public void Stress_RecordSet_Int32_Random_OneDifference_NotEquals()
    {
        int n = GetSizeOrDefault(@default: 1_000_000);
        Random random = new();

        RecordSet<int> left = new(capacity: n);
        RecordSet<int> right = new(capacity: n);

        // Add same random values to both collections
        for (int i = 0; i < n; i++)
        {
            int value = random.Next();
            left.Add(value);
            right.Add(value);
        }

        // Introduce one difference - add a value that doesn't exist in left
        int differentValue = random.Next();
        while (left.Contains(differentValue))
        {
            differentValue = random.Next();
        }
        right.Add(differentValue);

        // Act
        bool areEqual = left.Equals(right);

        // Assert
        Assert.IsFalse(areEqual, $"Collections should not be equal with one additional value {differentValue}");
    }

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
