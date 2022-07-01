// <copyright file="OptimalStringAlignmentTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Util.Tests;

public class OptimalStringAlignmentTests
{
    [TestCase("", "", 0)]
    [TestCase("", "a", 1)]
    [TestCase("a", "", 1)]
    [TestCase("FooBar", "FooBra", 1)]
    [TestCase("FooBar", "FoBar", 1)]
    [TestCase("FooBar", "FoooBar", 1)]
    [TestCase("FooBar", "FooCar", 1)]
    [TestCase("FooBar", "FooooBar", 2)]
    public void CalculateOptimalAlignment(string s1, string s2, int expected)
    {
        var actual = OptimalStringAlignment.Calculate(s1, s2);
        actual.Should().Be(expected);
    }
}