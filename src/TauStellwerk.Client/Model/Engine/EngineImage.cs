// <copyright file="EngineImage.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using TauStellwerk.Base;

namespace TauStellwerk.Client.Model;
public class EngineImage
{
    public EngineImage(string filename, int width)
    {
        Filename = filename;
        Width = width;
    }

    public string Filename { get; }

    public int Width { get; }

    public string Type => GetFileType().Type;

    public int Importance => GetFileType().Importance;

    public static ImmutableList<EngineImage> CreateFromImageDtoList(IList<ImageDto> images)
    {
        return images.Select(i => new EngineImage(i.Filename, i.Width)).ToImmutableList();
    }

    public static List<ImageDto> ToDtos(ImmutableList<EngineImage> list)
    {
        return list.Select(i => new ImageDto() { Filename = i.Filename, Width = i.Width }).ToList();
    }

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
