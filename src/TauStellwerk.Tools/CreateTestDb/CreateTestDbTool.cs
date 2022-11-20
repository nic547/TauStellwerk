// <copyright file="CreateTestDbTool.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using TauStellwerk.Base;
using TauStellwerk.Server.Database;
using TauStellwerk.Server.Database.Model;

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
        var tags = await CollectTags(engineList, context);

        foreach (var engineDtoChunk in engineList.Chunk(100))
        {
            foreach (var engineDto in engineDtoChunk)
            {
                var engine = new Engine()
                {
                    Tags = engineDto.Tags.Select(s => tags[s]).ToList(),
                };

                await engine.UpdateWith(engineDto, context);

                context.Add(engine);
                Console.WriteLine($"Created engine {engine.Name}");
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task<Dictionary<string, Tag>> CollectTags(IList<EngineFullDto> engines, StwDbContext dbContext)
    {
        var tags = engines.SelectMany(e => e.Tags).Distinct().ToList();
        var existingTags = await dbContext.Tags.ToListAsync();
        var tagsToInsert = tags.Except(existingTags.Select(t => t.Name)).Select(s => new Tag(0, s)).ToList();

        await dbContext.Tags.AddRangeAsync(tagsToInsert);
        await dbContext.SaveChangesAsync();

        existingTags.AddRange(tagsToInsert);
        return existingTags.ToDictionary(t => t.Name);
    }
}
