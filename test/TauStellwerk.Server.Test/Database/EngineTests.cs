// <copyright file="EngineTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TauStellwerk.Base.Dto;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.Database.Tests;

/// <summary>
/// Contains tests related to <see cref="Engine"/>.
/// </summary>
public class EngineTests : ContextTestBase
{
    [Test]
    public async Task EngineTagsAreNormalized()
    {
        EngineFullDto dto = new()
        {
            Tags = new List<string>
            {
                "\x00E4",
                "\x0061\x0308",
            },
        };

        Engine engine = new();
        var context = GetContext();

        await engine.UpdateWith(dto, context);
        await context.Engines.AddAsync(engine);
        await context.SaveChangesAsync();

        var tags = await context.Tags.ToListAsync();

        tags.Should().HaveCount(1);
    }
}
