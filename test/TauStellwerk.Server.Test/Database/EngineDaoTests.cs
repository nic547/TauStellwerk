// <copyright file="EngineDaoTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Data.Dao;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Test.Database;

public class EngineDaoTests : ContextTestBase
{
    /// <summary>
    /// Ensure that when loading an engine via the EngineDao the lastUsed-timestamp is updated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoadingEngineUpdatesLastUseDateTime()
    {
        Engine engine = new()
        {
            LastUsed = DateTime.Now,
        };

        await using (var context = GetContext())
        {
            context.Engines.Add(engine);
            await context.SaveChangesAsync();
        }

        await using (var context = GetContext())
        {
            var logger = Substitute.For<ILogger<EngineDao>>();
            var engineDao = new EngineDao(context, logger);

            _ = engineDao.GetEngine(engine.Id);
        }

        await using (var context = GetContext())
        {
            var savedEngine = await context.Engines.SingleOrDefaultAsync(e => e.Id == engine.Id);
            savedEngine!.LastUsed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Test]
    [TestCase("notathing", 0)]
    [TestCase("ABC", 1)]
    [TestCase("abc", 1)]
    [TestCase("C", 3)]
    [TestCase("CD", 2)]
    [TestCase("%' OR TRUE OR name LIKE '%A", 0)]
    [TestCase("tag", 3)]
    [TestCase("Tag", 3)]
    [TestCase("", 3)]
    public async Task TextSearchCanFindEngine(string searchTerm, int expectedCount)
    {
        var dao = GetDao();

        await InsertTestDataSet(dao);

        var searchResult = await dao.GetEngineList(searchTerm, 0, false, SortEnginesBy.Created, true);

        searchResult.Should().HaveCount(expectedCount);
    }

    [Test]
    public async Task TextSearchDoesntIncludeHidden()
    {
        var dao = GetDao();

        var engineDto = new EngineFullDto()
        {
            Name = "Testengine",
            IsHidden = true,
        };

        await dao.UpdateOrAdd(engineDto);

        var searchResult = await dao.GetEngineList("Test", 0, false, SortEnginesBy.Name, true);

        searchResult.Should().BeEmpty();
    }

    [Test]
    public async Task TextSearchCanIncludeHidden()
    {
        var dao = GetDao();

        var engineDto = new EngineFullDto()
        {
            Name = "TestEngine",
            IsHidden = true,
        };

        await dao.UpdateOrAdd(engineDto);

        var searchResult = await dao.GetEngineList("Test", 0, true, SortEnginesBy.Name, true);

        searchResult.Should().HaveCount(1);
    }

    [Test]
    public async Task TextSearchCanIncludeSemicolon()
    {
        var dao = GetDao();

        var engineDto = new EngineFullDto()
        {
            Name = ";;;",
            IsHidden = false,
        };

        await dao.UpdateOrAdd(engineDto);

        var searchResult = await dao.GetEngineList(";", 0, false, SortEnginesBy.Name, true);

        searchResult.Should().HaveCount(1);
    }

    [Test]
    public async Task TextSearchCanBeOrderedByName()
    {
        var dao = GetDao();

        await InsertTestDataSet(dao);

        var searchResult = await dao.GetEngineList("Lok", 0, false, SortEnginesBy.Name, false);

        searchResult.Should().HaveCount(3);
        searchResult[0].Name.Should().StartWith("A");
        searchResult[1].Name.Should().StartWith("B");
        searchResult[2].Name.Should().StartWith("C");
    }

    private static async Task InsertTestDataSet(EngineDao dao)
    {
        EngineFullDto dto1 = new()
        {
            Name = "ABC-Lok",
            Tags = new() { "Tag" },
        };

        EngineFullDto dto2 = new()
        {
            Name = "BCD-Lok",
            Tags = new() { "Tag" },
        };

        EngineFullDto dto3 = new()
        {
            Name = "CDE-Lok",
            Tags = new() { "Tag" },
        };

        await dao.UpdateOrAdd(dto1);
        await dao.UpdateOrAdd(dto2);
        await dao.UpdateOrAdd(dto3);
    }

    private EngineDao GetDao()
    {
        var loggerMock = Substitute.For<ILogger<EngineDao>>();
        return new EngineDao(GetContext(), loggerMock);
    }
}
