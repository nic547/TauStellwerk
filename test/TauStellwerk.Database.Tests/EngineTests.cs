// <copyright file="EngineTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Database.Tests;

/// <summary>
/// Contains tests related to <see cref="Engine"/>.
/// </summary>
public class EngineTests
{
    /// <summary>
    /// Test that after serializing and deserializing via json the "new" engine object has the same values as the "old" one.
    /// </summary>
    [Test]
    public void EnginesMatchAfterJson()
    {
        var expectedEngine = TestDataHelper.CreateTestEngine();
        var json = JsonSerializer.Serialize(expectedEngine);
        var resultEngine = JsonSerializer.Deserialize<Engine>(json);

        resultEngine.Should().BeEquivalentTo(expectedEngine);
    }
}
