// <copyright file="MultiDictionaryTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

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
        Assert.AreEqual("Deux", out1);

        mdict.TryRemoveFirst(2, out var out2);
        Assert.AreEqual("Zwo", out2);

        Assert.False(mdict.TryRemoveFirst(2, out _));
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

        Assert.AreEqual(3, list.Count);
        Assert.Contains("Eins", list);
        Assert.Contains("One", list);
        Assert.Contains("Uno", list);
    }

    /// <summary>
    /// Test that getting a nonexistent key returns false and the out parameter is null.
    /// </summary>
    [Test]
    public void TestGettingNonexistentKey()
    {
        var mdict = new MultiDictionary<int, object>();
        var success = mdict.TryGetAllValues(482001, out var list);

        Assert.False(success);
        Assert.IsNull(list);
    }
}
