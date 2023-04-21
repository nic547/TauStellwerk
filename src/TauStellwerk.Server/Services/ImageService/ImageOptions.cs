// <copyright file="ImageOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NetVips;

namespace TauStellwerk.Server.Services;

public abstract record ImageOptions(double Scale, string Suffix, string FileEnding)
{
    public string GetFilename(int id)
    {
        return $"{id}{Suffix}{FileEnding}";
    }
}

public record WebpOptions(double Scale, string Suffix, int Quality, int Effort) :
    ImageOptions(Scale, Suffix, ".webp");

public record JpegOptions(double Scale, string Suffix, int Quality) : ImageOptions(Scale, Suffix, ".jpeg");

public record AvifOptions(double Scale, string Suffix, int Quality, int Effort) : ImageOptions(Scale, Suffix, ".avif")
{
    public Enums.ForeignHeifCompression HeifCompression { get; } = Enums.ForeignHeifCompression.Av1;
}