namespace System.Collections.Tests.Generic;

[TestClass]
public class EqualityCollectionComparerTests
{
    [TestMethod]
    public void EqualityList_DefaultConstructor_UsesDefaultComparer()
    {
        // arrange
        global::System.Collections.IRecordCollectionComparer original = RecordCollectionComparer.Default;
        TestRecordCollectionComparer overrideComparer = new();
        RecordCollectionComparer.Default = overrideComparer;

        try
        {
            // act
            EqualityList<int> list = [];

            // assert
            Assert.AreSame(overrideComparer, list.Comparer);
        }
        finally
        {
            RecordCollectionComparer.Default = original;
        }
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
        global::System.Collections.IRecordCollectionComparer original = RecordCollectionComparer.Default;
        TestRecordCollectionComparer overrideComparer = new();
        RecordCollectionComparer.Default = overrideComparer;

        try
        {
            // act
            EqualityDictionary<int, string> dictionary = new();

            // assert
            Assert.AreSame(overrideComparer, dictionary.Comparer);
        }
        finally
        {
            RecordCollectionComparer.Default = original;
        }
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
