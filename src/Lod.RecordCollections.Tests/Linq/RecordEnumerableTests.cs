namespace Lod.RecordCollections.Tests.Linq;

[TestClass]
public class RecordEnumerableTests
{
    #region ToRecordList

    [TestMethod]
    public void ToRecordList_ValidEnumerable_ReturnsRecordList()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));

        // act
        RecordList<Number> recordList = enumerable.ToRecordList();

        // assert
        Assert.IsNotNull(recordList);
        Assert.HasCount(enumerable.Count(), recordList);
    }

    [TestMethod]
    public void ToRecordList_NullEnumerable_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordList<Number>? recordList = null;
        Exception? exception = null;

        // act
        try
        {
            recordList = enumerable.ToRecordList();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordList);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    #endregion

    #region ToRecordDictionary
    #pragma warning disable IDE0039 // Use local function

    [TestMethod]
    public void ToRecordDictionary_ValidParams_ReturnsRecordDictionary()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));

        // act
        RecordDictionary<int, Number> recordDictionary = enumerable.ToRecordDictionary(kv => kv.Value);

        // assert
        Assert.IsNotNull(recordDictionary);
        Assert.HasCount(enumerable.Count(), recordDictionary);
    }

    [TestMethod]
    public void ToRecordDictionary_NullEnumerableAndValidKeySelector_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordDictionary<int, Number>? recordDictionary = null;
        Func<Number, int> keySelector = r => r.Value;
        Exception? exception = null;

        // act
        try
        {
            recordDictionary = enumerable.ToRecordDictionary(keySelector);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordDictionary);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordDictionary_ValidEnumerableAndNullKeySelector_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));
        RecordDictionary<int, Number>? recordDictionary = null;
        Func<Number, int> keySelector = null!;
        Exception? exception = null;

        // act
        try
        {
            recordDictionary = enumerable.ToRecordDictionary(keySelector);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordDictionary);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordDictionary_NullEnumerableAndValidKeySelectorAndValidElementSelector_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordDictionary<int, Number>? recordDictionary = null;
        Func<Number, int> keySelector = r => r.Value;
        Func<Number, Number> elementSelector = r => r with { };
        Exception? exception = null;

        // act
        try
        {
            recordDictionary = enumerable.ToRecordDictionary(keySelector, elementSelector);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordDictionary);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordDictionary_ValidEnumerableAndNullKeySelectorAndValidElementSelector_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(0, 10).Select(i => new Number(i));
        RecordDictionary<int, Number>? recordDictionary = null;
        Func<Number, int> keySelector = null!;
        Func<Number, Number> elementSelector = r => r with { };
        Exception? exception = null;

        // act
        try
        {
            recordDictionary = enumerable.ToRecordDictionary(keySelector, elementSelector);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordDictionary);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordDictionary_NullEnumerableValidKeySelectorAndValidElementSelector_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(0, 10).Select(i => new Number(i));
        RecordDictionary<int, Number>? recordDictionary = null;
        Func<Number, int> keySelector = r => r.Value;
        Func<Number, Number> elementSelector = null!;
        Exception? exception = null;

        // act
        try
        {
            recordDictionary = enumerable.ToRecordDictionary(keySelector, elementSelector);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordDictionary);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    #pragma warning restore IDE0039 // Use local function
    #endregion

    #region ToRecordSet

    [TestMethod]
    public void ToRecordSet_ValidEnumerable_ReturnsRecordSet()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));

        // act
        RecordSet<Number> recordSet = enumerable.ToRecordSet();

        // assert
        Assert.IsNotNull(recordSet);
        Assert.HasCount(enumerable.Count(), recordSet);
    }

    [TestMethod]
    public void ToRecordSet_NullEnumerable_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordSet<Number>? recordSet = null;
        Exception? exception = null;

        // act
        try
        {
            recordSet = enumerable.ToRecordSet();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordSet);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    #endregion

    #region ToRecordStack

    [TestMethod]
    public void ToRecordStack_ValidEnumerable_ReturnsRecordStack()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));

        // act
        RecordStack<Number> recordStack = enumerable.ToRecordStack();

        // assert
        Assert.IsNotNull(recordStack);
        Assert.HasCount(enumerable.Count(), recordStack);
    }

    [TestMethod]
    public void ToRecordStack_NullEnumerable_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordStack<Number>? recordStack = null;
        Exception? exception = null;

        // act
        try
        {
            recordStack = enumerable.ToRecordStack();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordStack);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordStack_ValidEnumerable_PreservesOrder()
    {
        // arrange
        IEnumerable<int> enumerable = [1, 2, 3, 4, 5];

        // act
        RecordStack<int> recordStack = enumerable.ToRecordStack();

        // assert
        Assert.IsNotNull(recordStack);
        Assert.AreEqual(5, recordStack.Count);
        // Stack is LIFO, so last element should be on top
        Assert.AreEqual(5, recordStack.Peek());
    }

    #endregion

    #region ToRecordQueue

    [TestMethod]
    public void ToRecordQueue_ValidEnumerable_ReturnsRecordQueue()
    {
        // arrange
        IEnumerable<Number> enumerable = Enumerable.Range(1, 10).Select(i => new Number(i));

        // act
        RecordQueue<Number> recordQueue = enumerable.ToRecordQueue();

        // assert
        Assert.IsNotNull(recordQueue);
        Assert.HasCount(enumerable.Count(), recordQueue);
    }

    [TestMethod]
    public void ToRecordQueue_NullEnumerable_ThrowArgumentNullException()
    {
        // arrange
        IEnumerable<Number> enumerable = null!;
        RecordQueue<Number>? recordQueue = null;
        Exception? exception = null;

        // act
        try
        {
            recordQueue = enumerable.ToRecordQueue();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // assert
        Assert.IsNull(recordQueue);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType<ArgumentNullException>(exception);
    }

    [TestMethod]
    public void ToRecordQueue_ValidEnumerable_PreservesOrder()
    {
        // arrange
        IEnumerable<int> enumerable = [1, 2, 3, 4, 5];

        // act
        RecordQueue<int> recordQueue = enumerable.ToRecordQueue();

        // assert
        Assert.IsNotNull(recordQueue);
        Assert.AreEqual(5, recordQueue.Count);
        // Queue is FIFO, so first element should be at front
        Assert.AreEqual(1, recordQueue.Peek());
    }

    #endregion
}
