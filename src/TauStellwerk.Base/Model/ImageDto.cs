// <copyright file="ImageDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace TauStellwerk.Base.Model
{
    public class ImageDto
    {
        public string Filename { get; set; } = string.Empty;

        public int Width { get; set; }

        [JsonIgnore]
        public string Type => GetFileType().Type;

        [JsonIgnore]
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
