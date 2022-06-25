// <copyright file="FunctionDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Base.Model;

namespace TauStellwerk.Base.Dto;

public class FunctionDto
{
    public FunctionDto(byte number, string name, int duration)
    {
        Number = number;
        Name = name;
        Duration = duration;
    }

    /// <summary>
    /// Gets or sets the dcc function number this function has (F0 or F18 for example).
    /// </summary>
    public byte Number { get; set; }

    /// <summary>
    /// Gets or sets the name that this function should have.
    /// </summary>
    public string Name { get; set; }

    public State State { get; set; } = State.Off;

    /// <summary>
    /// Gets or sets the duration of a function in milliseconds. Zero or lower represent latching functions (infinite duration).
    /// </summary>
    public int Duration { get; set; } = -1;

    public override string ToString() => $"F{Number} - {Name}";
}