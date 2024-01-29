// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Util.Extensions;

namespace TauStellwerk.Util.Tests;

public class StringExtensionTests
{
    [TestCase("", 2, "")]
    [TestCase("AAA", 1, "A")]
    [TestCase("ABC", 4, "ABC")]
    [TestCase("𝄞𝄞", 1, "𝄞")]
    [TestCase("👩‍👩‍👦", 1, "👩‍👩‍👦")] // This is a "Family" emoji with two women and a boy, connected with zero-width joiners.
    [TestCase("ABCD", -2, "")]
    public void RightExtensionTest(string input, int numberOfChars, string expected)
    {
        var output = input.Left(numberOfChars);
        output.Should().Be(expected);
    }
}
