using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RepeatTestMethodAttribute : Attribute, ITestDataSource
{
    public RepeatTestMethodAttribute(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        Count = count;
    }

    public int Count { get; }

    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        for (int i = 0; i < Count; i++)
        {
            yield return Array.Empty<object?>();
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => $"{methodInfo.Name} (Repeated)";
}
