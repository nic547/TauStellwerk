// <copyright file="TurnoutDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Base.Model;

namespace TauStellwerk.Base.Dto;

public class TurnoutDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsInverted { get; set; }

    public int Address { get; set; }

    public TurnoutKind Kind { get; set; }

    public State State { get; set; }
}