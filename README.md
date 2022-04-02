# Record Collections
This library provides a simple implementation of generic collections which implement equality comparison for record types.

## Records
Beginning with C# 9, you use the record keyword to define a reference type that provides built-in functionality for encapsulating data.
C# 10 allows the record class syntax as a synonym to clarify a reference type, and record struct to define a value type with similar functionality.
You can create record types with immutable properties by using positional parameters or standard property syntax.
Read more about records at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record

## Record Collections
C# records provide value type equality comparison of reference types.
The default implementation of records looks at the HashCode of each property in the object.
This works great for structural data types; however, collections in .NET are all reference types which provide a unique instance hash code.
In order to include collections on records, you need a collection type which provides value based equity for the collection hash rather than the instance hash code.
Many immutable (and mutable) records may contain arrays, lists, or other collections of sub-records; however, there is no record friendly implimentation of any collection type in .NET Framework or .NET Core.

## Usage
The library places a Record\* collection type in the same namespace as it's underlying implementation.
  - RecordList\<T\>, which uses an underlying List\<T\> both reside in the System.Collections.Generic namespace.
  
Each of the wrapped types should expose as many of the underlying methods as possible.
The collections should behave the same as the wrapped class as well. Updating a RecordList<T> while enumerating will still throw an `InvalidOperationException`, while the RecordArray will not.

## ⚠ Drawbacks & Warnings
The current implementation of this library loops through each record in the collection to determine the current hash code of the collection.
This can become very expensive when doing equality comparisons with large collections.

Record\* collections also attempt to clone the underlying collection elements every time the record is cloned.
If an element is not value/record like the original element is retained and may be modified between seperate record collections.

The library is intended to provide a default but extendable implementation of common collections. It works great for data models with small collections; however, when used with large data sets preformance will start to deteriorate.

## Examples
```
record NumberInfo
{
    public IList<int> Numbers { get; init; }
}

var nums1 = new NumberInfo()
{
    Numbers = new List<int>() { 1, 2, 3, },
};
var nums2 = new NumberInfo()
{
    Numbers = new List<int>() { 1, 2, 3, },
};
Console.WriteLine(nums1 == nums2); // Outputs "False" as the two lists return the instance hash code.

var nums3 = new NumberInfo()
{
    Numbers = new RecordList<int>() { 1, 2, 3, },
};
var nums4 = new NumberInfo()
{
    Numbers = new RecordList<int>() { 1, 2, 3, },
};
Console.WriteLine(nums3 == nums4); // Outputs "True" as the two record lists return the aggregated hash of the underlying elements.
```
