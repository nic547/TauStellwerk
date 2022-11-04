// <copyright file="EngineDaoTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TauStellwerk.Server.Dao;
using TauStellwerk.Server.Database.Model;

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
            var logger = new Mock<ILogger<EngineDao>>();
            var engineDao = new EngineDao(context, logger.Object);

            _ = engineDao.GetEngine(engine.Id);
        }

        await using (var context = GetContext())
        {
            var savedEngine = await context.Engines.SingleOrDefaultAsync(e => e.Id == engine.Id);
            savedEngine!.LastUsed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}
