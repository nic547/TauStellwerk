// <copyright file="CreateTestDbTool.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using TauStellwerk.Server.Data;
using TauStellwerk.Server.Data.Model;
using TauStellwerk.Server.Database;

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

        foreach (var engineDtoChunk in engineList.Chunk(100))
        {
            foreach (var engineDto in engineDtoChunk)
            {
                var engine = new Engine();
                engine.UpdateWith(engineDto);

                context.Add(engine);
                Console.WriteLine($"Created engine {engine.Name}");
            }

            await context.SaveChangesAsync();
        }
    }
}
