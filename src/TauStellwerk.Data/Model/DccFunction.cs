﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations.Schema;
using TauStellwerk.Base.Dto;

namespace TauStellwerk.Data.Model;

public class DccFunction
{
    public DccFunction(byte number, string name, int duration)
    {
        Number = number;
        Name = name;
        Duration = duration;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the dcc function number this function has (F0 or F18 for example).
    /// </summary>
    public byte Number { get; set; }

    public string Name { get; set; }

    public int Duration { get; set; }

    public FunctionDto ToFunctionDto()
    {
        return new(Number, Name, Duration);
    }
}
