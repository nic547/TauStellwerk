// <copyright file="CreateTestDbTool.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Database;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Tools.CreateTestDb;

public static class CreateTestDbTool
{
    public static async Task Run(CreateTestDbOptions options)
    {
        var contextOptions =
            new DbContextOptionsBuilder<StwDbContext>().UseSqlite($"Filename={options.Filename};cache=shared");
        var context = new StwDbContext(contextOptions.Options);
        await context.Database.MigrateAsync();

        var engineList = EngineDtoGenerator.GetEngineFullDtos(options.Count);

        foreach (var engineDto in engineList)
        {
            var engine = new Engine();
            await engine.UpdateWith(engineDto, context);
            context.Add(engine);
            await context.SaveChangesAsync();

            Console.WriteLine($"Created engine {engine.Name}");
        }
    }
}
