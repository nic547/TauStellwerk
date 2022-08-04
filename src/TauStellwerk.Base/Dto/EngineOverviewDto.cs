// <copyright file="EngineOverviewDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Base;

public class EngineOverviewDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public List<ImageDto> Images { get; set; } = new();

    public DateTime LastUsed { get; set; }

    public DateTime Created { get; set; }

    public bool IsHidden { get; set; }
}