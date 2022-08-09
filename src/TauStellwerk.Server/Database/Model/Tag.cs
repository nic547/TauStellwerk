// <copyright file="Tag.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace TauStellwerk.Server.Database.Model;

[Index(nameof(Name), IsUnique = true)]
public class Tag
{
    public Tag(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public List<Engine> Engines { get; } = new();

    public static async Task<List<Tag>> GetTagsFromStrings(List<string> tags, StwDbContext dbContext)
    {
        var result = new List<Tag>();

        result.AddRange(await dbContext.Tags.Where(t => tags.Contains(t.Name)).ToListAsync());

        tags.RemoveAll(t => result.Select(x => x.Name).Contains(t));

        foreach (var missingTag in tags)
        {
            result.Add(new Tag(0, missingTag));
        }

        return result;
    }
}