namespace Lod.RecordCollections.Tests.Collections.Generic;

[TestClass]
public class EqualityCollectionComparerTests
{
    [TestInitialize]
    public void SetUp()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = new RecordCollectionComparer();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [TestMethod]
    [RepeatTestMethod(3)]
    public void EqualityList_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = comparer;
#pragma warning restore CS0618 // Type or member is obsolete

        // Act
        EqualityList<int> list = [];

        // Assert
        Assert.AreSame(comparer, list.Comparer);
    }

    [TestMethod]
    public void EqualityList_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        EqualityList<int> list = new(comparer);

        // assert
        Assert.AreSame(comparer, list.Comparer);
    }

    [TestMethod]
    public void EqualityList_Operators_UseTypedEquals()
    {
        OperatorAwareEqualityList left = new([92, 117]);
        OperatorAwareEqualityList right = new([92, 117]);

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
    [RepeatTestMethod(3)]
    public void EqualityDictionary_DefaultConstructor_UsesDefaultComparer()
    {
        // Arrange
        TestRecordCollectionComparer comparer = new();
#pragma warning disable CS0618 // Type or member is obsolete
        RecordCollectionComparer.Default = comparer;
#pragma warning restore CS0618 // Type or member is obsolete

        // Act
        EqualityDictionary<int, string> dictionary = [];

        // Assert
        Assert.AreSame(comparer, dictionary.Comparer);
    }

    [TestMethod]
    public void EqualityDictionary_CustomComparerConstructor_UsesProvidedComparer()
    {
        // arrange
        TestRecordCollectionComparer comparer = new();

        // act
        EqualityDictionary<int, string> dictionary = new(comparer);

        // assert
        Assert.AreSame(comparer, dictionary.Comparer);
    }

    [TestMethod]
    public void EqualityDictionary_Operators_UseTypedEquals()
    {
        OperatorAwareEqualityDictionary left = OperatorAwareEqualityDictionary.Create();
        OperatorAwareEqualityDictionary right = OperatorAwareEqualityDictionary.Create();

        left.Reset();
        _ = left == right;
        Assert.IsTrue(left.TypedEqualsCalled);
        Assert.IsFalse(left.ObjectEqualsCalled);

        left.Reset();
        _ = left != right;
        Assert.IsTrue(left.TypedEqualsCalled);
        Assert.IsFalse(left.ObjectEqualsCalled);
    }

    private sealed class OperatorAwareEqualityList(IEnumerable<int> values) : EqualityList<int>(values)
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

        public override bool Equals(EqualityList<int>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    private sealed class OperatorAwareEqualityDictionary : EqualityDictionary<int, string>
    {
        private OperatorAwareEqualityDictionary(Dictionary<int, string> values) : base(values) { }

        public bool TypedEqualsCalled { get; private set; }
        public bool ObjectEqualsCalled { get;private set; }

        public static OperatorAwareEqualityDictionary Create() =>
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

        public override bool Equals(EqualityDictionary<int, string>? other)
        {
            TypedEqualsCalled = true;
            return base.Equals(other);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
