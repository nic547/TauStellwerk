// <copyright file="ClampExtensionTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Util.Tests
{
    public class ClampExtensionTests
    {
        [TestCase(-1, 1, 128, 1)]
        [TestCase(0, 1, int.MaxValue, 1)]
        [TestCase(65, 1, 64, 64)]
        [TestCase(63, 1, 64, 63)]
        [TestCase(int.MaxValue, 1, 64, 64)]
        [Test]
        public void UpperLowerClampTest(int value, int lower, int upper, int expected)
        {
            var result = value.Clamp(lower, upper);
            result.Should().Be(expected);
        }

        [TestCase(1, 126, 1)]
        [TestCase(128, 126, 126)]
        [TestCase(0, 126, 0)]
        [TestCase(-1, 126, 0)]
        [Test]
        public void UpperClampTest(int value, int upper, int expected)
        {
            var result = value.Clamp(upper);
            result.Should().Be(expected);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(-1, 0)]
        [Test]
        public void ClampTest(int value, int expected)
        {
            var result = value.Clamp();
            result.Should().Be(expected);
        }
    }
}
