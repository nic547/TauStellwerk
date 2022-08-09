// <copyright file="Engine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Base;

namespace TauStellwerk.Server.Database.Model;

/// <summary>
/// A choo-choo, in this context generally understood to be smaller than real-live-sized.
/// </summary>
[Index(nameof(LastUsed))]
[Index(nameof(Created))]
public class Engine
{
    /// <summary>
    /// Gets or sets the name of the choo-choo.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Id of the Engine in the database system.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the DCC Address of an Engine. Is not necessarily unique.
    /// </summary>
    public ushort Address { get; set; }

    /// <summary>
    /// Gets or sets the amount of speed steps the decoder supports.
    /// </summary>
    public byte SpeedSteps { get; set; }

    /// <summary>
    /// Gets or sets the top speed of the real thing.
    /// </summary>
    public int TopSpeed { get; set; }

    /// <summary>
    /// Gets a list of functions a decoder offers.
    /// </summary>
    public List<DccFunction> Functions { get; init; } = new();

    /// <summary>
    /// Gets a list of strings that describe an engineOverview. These might be alternative names, manufacturers, the owner etc, basically
    /// everything one might search for if the exact name is unknown.
    /// </summary>
    public List<Tag> Tags { get; init; } = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ECoSEngineData? ECoSEngineData { get; init; }

    public DateTime? LastImageUpdate { get; set; }

    public List<EngineImage> Images { get; init; } = new();

    public DateTime LastUsed { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public bool IsHidden { get; set; }

    public override string ToString()
    {
        return $"'{Name}'";
    }

    public EngineOverviewDto ToEngineDto()
    {
        return new EngineOverviewDto()
        {
            Id = Id,
            Name = Name,
            Images = Images.Select(i => i.ToImageDto()).ToList(),
            Tags = Tags.Select(t => t.Name).ToList(),
            LastUsed = LastUsed,
            Created = Created,
            IsHidden = IsHidden,
        };
    }

    public EngineFullDto ToEngineFullDto()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Images = Images.Select(i => i.ToImageDto()).ToList(),
            Tags = Tags.Select(t => t.Name).ToList(),
            LastUsed = LastUsed,
            Created = Created,
            TopSpeed = TopSpeed,
            Address = Address,
            Functions = Functions.Select(f => f.ToFunctionDto()).ToList(),
            IsHidden = IsHidden,
        };
    }

    public async Task UpdateWith(EngineFullDto engineDto, StwDbContext dbContext)
    {
        Name = engineDto.Name.Normalize();
        TopSpeed = engineDto.TopSpeed;
        Address = engineDto.Address;

        var incomingTags = engineDto.Tags.Select(t => t.Normalize()).Distinct().ToList();

        Tags.RemoveAll(t => !incomingTags.Contains(t.Name));

        var newTags = incomingTags.Where(newTag => Tags.All(existingTag => existingTag.Name != newTag)).ToList();
        Tags.AddRange(await Tag.GetTagsFromStrings(newTags, dbContext));

        UpdateFunctions(engineDto.Functions);

        LastUsed = engineDto.LastUsed;
        Created = engineDto.Created;
        IsHidden = engineDto.IsHidden;
    }

    private void UpdateFunctions(List<FunctionDto> updateFunctions)
    {
        Functions.RemoveAll(existingFunction => updateFunctions.All(updateFunction => updateFunction.Number != existingFunction.Number));

        foreach (var updateFunction in updateFunctions)
        {
            var newFunctionName = updateFunction.Name.Normalize();
            var matchingExistingFunction = Functions.SingleOrDefault(f => f.Number == updateFunction.Number);

            if (matchingExistingFunction == null)
            {
                Functions.Add(new DccFunction(updateFunction.Number, newFunctionName, updateFunction.Duration));
                continue;
            }

            matchingExistingFunction.Name = newFunctionName;
            matchingExistingFunction.Duration = updateFunction.Duration;
        }
    }
}