﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using TauStellwerk.Data;
using TauStellwerk.Data.Model;

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
