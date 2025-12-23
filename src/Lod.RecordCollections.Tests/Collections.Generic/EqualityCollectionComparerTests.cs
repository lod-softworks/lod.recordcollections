namespace System.Collections.Tests.Generic;

[TestClass]
public class EqualityCollectionComparerTests
{
    [TestMethod]
    public void EqualityList_DefaultConstructor_UsesDefaultComparer()
    {
        // arrange
        global::System.Collections.IRecordCollectionComparer defaultComparer = RecordCollectionComparer.Default;

        // act
        EqualityList<int> list = [];

        // assert
        Assert.AreSame(defaultComparer, list.Comparer);
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
    public void EqualityDictionary_DefaultConstructor_UsesDefaultComparer()
    {
        // arrange
        global::System.Collections.IRecordCollectionComparer defaultComparer = RecordCollectionComparer.Default;

        // act
        EqualityDictionary<int, string> dictionary = new();

        // assert
        Assert.AreSame(defaultComparer, dictionary.Comparer);
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
}
