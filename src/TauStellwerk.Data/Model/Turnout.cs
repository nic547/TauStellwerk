// <copyright file="Turnout.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TauStellwerk.Base;

namespace TauStellwerk.Data.Model;

public class Turnout
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public bool IsInverted { get; set; }

    public int Address { get; set; }

    public TurnoutKind Kind { get; set; }

    [NotMapped]
    public State State { get; set; }

    public static Turnout FromDto(TurnoutDto dto)
    {
        return new Turnout
        {
            Id = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            IsInverted = dto.IsInverted,
            Kind = dto.Kind,
        };
    }

    public TurnoutDto ToDto()
    {
        return new TurnoutDto
        {
            Id = Id,
            Name = Name,
            Address = Address,
            IsInverted = IsInverted,
            Kind = Kind,
            State = State,
        };
    }
}