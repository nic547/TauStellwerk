// <copyright file="ClampExtensionTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Util.Tests;

public class ClampExtensionTests
{
    [TestCase(-1, 1, 128, 1)]
    [TestCase(0, 1, int.MaxValue, 1)]
    [TestCase(65, 1, 64, 64)]
    [TestCase(63, 1, 64, 63)]
    [TestCase(int.MaxValue, 1, 64, 64)]
    [TestCase(200L, 0L, 100L, 100L)]
    [TestCase(float.MaxValue, 0.1f, 0.2f, 0.2f)]
    [Test]
    public void UpperLowerClampTest<T>(T value, T lower, T upper, T expected)
    where T : INumber<T>
    {
        var result = value.Clamp(lower, upper);
        result.Should().Be(expected);
    }

    [TestCase(1, 126, 1)]
    [TestCase(128, 126, 126)]
    [TestCase(0, 126, 0)]
    [TestCase(-1, 126, 0)]
    [Test]
    public void UpperClampTest<T>(T value, T upper, T expected)
    where T : INumber<T>
    {
        var result = value.Clamp(upper);
        result.Should().Be(expected);
    }

    [TestCase((sbyte)0, (sbyte)0)]
    [TestCase((sbyte)1, (sbyte)1)]
    [TestCase((sbyte)-1, (sbyte)0)]
    [TestCase((short)0, (short)0)]
    [TestCase((short)1, (short)1)]
    [TestCase((short)-1, (short)0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(-1, 0)]
    [TestCase(0L, 0L)]
    [TestCase(1L, 1L)]
    [TestCase(-1L, 0L)]
    [TestCase(-0.1f, 0f)]
    [TestCase(-4.56, 0)]
    [TestCase(-3.14156D, 0D)]
    [Test]
    public void ClampTests<T>(T value, T expected)
    where T : INumber<T>
    {
        var result = value.Clamp();
        result.Should().Be(expected);
    }
}
