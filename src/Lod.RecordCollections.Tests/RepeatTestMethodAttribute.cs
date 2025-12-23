using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RepeatTestMethodAttribute : TestMethodAttribute
{
    public RepeatTestMethodAttribute(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        Count = count;
    }

    public int Count { get; }

    public override TestResult[] Execute(ITestMethod testMethod)
    {
        List<TestResult> results = new(Count);

        for (int i = 0; i < Count; i++)
        {
            TestResult result = testMethod.Invoke(null);
            results.Add(result);

            if (result.Outcome != UnitTestOutcome.Passed)
            {
                break;
            }
        }

        return results.ToArray();
    }
}
