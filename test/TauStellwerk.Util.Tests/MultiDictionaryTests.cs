// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Util.Tests;

/// <summary>
/// Tests related to <see cref="MultiDictionary{TKey,TValue}"/>.
/// </summary>
public class MultiDictionaryTests
{
    /// <summary>
    /// Test that the MultiDictionary can actually handle multiple values per key.
    /// </summary>
    [Test]
    public void AddingToSameKeyDoesNotThrow()
    {
        var mdict = new MultiDictionary<int, string>();
        Assert.DoesNotThrow(() => mdict.Add(1, "Eins"));
        Assert.DoesNotThrow(() => mdict.Add(1, "One"));
        Assert.DoesNotThrow(() => mdict.Add(1, "Uno"));
    }

    /// <summary>
    /// Test that added values get returned in the correct order and false is returned when no value is left.
    /// </summary>
    [Test]
    public void RemoveValuesFromSameKey()
    {
        var mdict = new MultiDictionary<int, string>();
        mdict.Add(2, "Deux");
        mdict.Add(2, "Zwo");

        mdict.TryRemoveFirst(2, out var out1);
        out1.Should().Be("Deux");

        mdict.TryRemoveFirst(2, out var out2);
        out2.Should().Be("Zwo");

        mdict.TryRemoveFirst(2, out _).Should().BeFalse();
    }

    /// <summary>
    /// Check that TryGetAllValues actually gets all values.
    /// </summary>
    [Test]
    public void TestMultiDictGetsAllValues()
    {
        var mdict = new MultiDictionary<int, string>();
        mdict.Add(1, "Eins");
        mdict.Add(1, "One");
        mdict.Add(1, "Uno");
        mdict.Add(2, "Deux");
        mdict.Add(2, "Zwo");

        _ = mdict.TryGetAllValues(1, out var immutableList);
        immutableList.Should().NotBeNull();

        var list = immutableList!.ToList();

        list.Count.Should().Be(3);
        list.Should().Contain("Eins");
        list.Should().Contain("One");
        list.Should().Contain("Uno");
    }

    /// <summary>
    /// Test that getting a nonexistent key returns false and the out parameter is null.
    /// </summary>
    [Test]
    public void TestGettingNonexistentKey()
    {
        var mdict = new MultiDictionary<int, object>();
        var success = mdict.TryGetAllValues(482001, out var list);

        success.Should().BeFalse();
        list.Should().BeNull();
    }
}
