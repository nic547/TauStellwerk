// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.


using NUnit.Framework;

namespace TauStellwerk.Util.Tests;

/// <summary>
/// Tests for <see cref="CounterDictionary"/>.
/// </summary>
[Parallelizable(ParallelScope.All)]
public class CounterDictionaryTests
{
    [TestCase(new int[] { }, null)]
    [TestCase(new[] { 1 }, 1d)]
    [TestCase(new[] { 1, 1, 1 }, 1d)]
    [TestCase(new[] { 1, 2 }, 1.5d)]
    [TestCase(new[] { 1, 2, 1, 2 }, 1.5d)]
    [TestCase(new[] { 1, 1, 1, 3 }, 1.5d)]
    public void AverageCaseTest(int[] values, double? result)
    {
        var ct = CounterFromArray(values);
        Assert.AreEqual(result, ct.Average());
    }

    [TestCase(new int[] { }, 0)]
    [TestCase(new[] { 0 }, 1)]
    [TestCase(new[] { 2 }, 1)]
    [TestCase(new[] { 2, 2, 1, 3, 1929 }, 5)]
    public void CountTest(int[] values, long result)
    {
        var ct = CounterFromArray(values);
        Assert.AreEqual(result, ct.Count());
    }

    [Test]
    public void PercentileTest()
    {
        var ct = new CounterDictionary();
        for (var i = 1; i <= 100_000; i++)
        {
            ct.Increment(i);
        }

        Assert.AreEqual(90_000, ct.Get90ThPercentile());
        Assert.AreEqual(95_000, ct.Get95ThPercentile());
        Assert.AreEqual(99_000, ct.Get99ThPercentile());
    }

    [Test]
    public void PercentilesOnEmptyNull()
    {
        var ct = new CounterDictionary();
        Assert.IsNull(ct.Get90ThPercentile());
    }

    [Test]
    public void CombineTest()
    {
        var ct = SimpleTestCounterDictionary();
        var ct2 = new CounterDictionary();
        for (var i = 0; i < 5; i++)
        {
            ct.Increment(3);
        }

        ct.Combine(ct2);

        Assert.AreEqual(5, ct.GetByKey(3));
    }

    private static CounterDictionary SimpleTestCounterDictionary()
    {
        var ct = new CounterDictionary();
        ct.Increment(1);
        ct.Increment(1);
        ct.Increment(1);
        ct.Increment(2);
        ct.Increment(2);
        ct.Increment(2);

        return ct;
    }

    private static CounterDictionary CounterFromArray(int[] array)
    {
        var ct = new CounterDictionary();
        foreach (var value in array)
        {
            ct.Increment(value);
        }

        return ct;
    }
}
