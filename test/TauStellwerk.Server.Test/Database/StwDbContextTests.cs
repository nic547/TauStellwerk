// <copyright file="StwDbContextTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TauStellwerk.Server.Data.Model;

namespace TauStellwerk.Test.Database;

/// <summary>
/// Contains tests related to the DbContext of the application.
/// </summary>
public class StwDbContextTests : ContextTestBase
{
    /// <summary>
    /// Test if a engine saved will be equal to the one loaded from it in a different dbContext.
    /// </summary>
    [Test]
    public void SaveAndLoadTest()
    {
        var originalEngine = TestDataHelper.CreateTestEngine();
        var context = GetContext();
        context.Engines.Add(originalEngine);
        context.SaveChanges();

        var loadContext = GetContext();
        var loadedEngine = loadContext.Engines
            .Include(x => x.Functions)
            .Single();

        loadedEngine.Should().NotBeSameAs(originalEngine);
        loadedEngine.Should().BeEquivalentTo(originalEngine);
    }

    /// <summary>
    /// Test that the testdata can be properly saved.
    /// </summary>
    [Test]
    public void InsertMultipleEngines()
    {
        var testData = TestDataHelper.CreateTestEngineList();
        var context = GetContext();
        context.Engines.AddRange(testData);
        context.SaveChanges();

        var testContext = GetContext();
        var loadedEngines = testContext.Engines;

        Assert.AreEqual(testData.Count, loadedEngines.Count());
    }

    /// <summary>
    /// Test that updates to an engine get saved properly and we for example don't get a second engine inserted instead.
    /// </summary>
    [Test]
    public void UpdatePersistsTest()
    {
        var testFunction = new DccFunction(1, "Headlights", -1);

        var originalEngine = TestDataHelper.CreateTestEngine();
        var context = GetContext();
        context.Engines.Add(originalEngine);
        context.SaveChanges();

        var updateContext = GetContext();
        var updateEngine = updateContext.Engines.Include(x => x.Functions).Single();
        updateEngine.Functions.Clear();
        updateEngine.Functions.Add(testFunction);
        updateContext.SaveChanges();

        var testContext = GetContext();
        var testEngine = testContext.Engines.Include(x => x.Functions).Single();

        testEngine.Should().NotBeSameAs(updateEngine);
        testEngine.Should().BeEquivalentTo(updateEngine);
    }
}
