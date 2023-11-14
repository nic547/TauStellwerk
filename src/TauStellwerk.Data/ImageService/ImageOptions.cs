// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using NetVips;

namespace TauStellwerk.Data.ImageService;

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
