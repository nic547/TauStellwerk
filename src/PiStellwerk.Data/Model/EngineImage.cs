// <copyright file="EngineImage.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PiStellwerk.Data
{
    public class EngineImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Filename { get; set; } = string.Empty;

        public bool IsGenerated { get; set; }

        public int Width { get; set; }

        public string Type => GetFileType().Type;

        public int Importance => GetFileType().Importance;

        private (string Type, int Importance) GetFileType()
        {
            return Filename.Split('.').Last() switch
            {
                null => (string.Empty, int.MaxValue),
                "jpg" or "jpeg" => ("image/jpeg", 4),
                "webp" => ("image/webp", 3),
                "avif" => ("image/avif", 2),
                _ => throw new InvalidOperationException($"Unknown file ending {Filename}"),
            };
        }
    }
}
