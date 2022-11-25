// <copyright file="EngineImage.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TauStellwerk.Base;

namespace TauStellwerk.Server.Database.Model;

public class EngineImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Filename { get; set; } = string.Empty;

    public int Width { get; set; }

    public ImageDto ToImageDto()
    {
        return new ImageDto(Filename, Width);
    }
}