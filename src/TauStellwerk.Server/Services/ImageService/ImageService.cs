// <copyright file="ImageService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NetVips;
using TauStellwerk.Base;
using TauStellwerk.Server.Data;
using TauStellwerk.Server.Data.Model;

namespace TauStellwerk.Server.Services;

public class ImageService
{
    private static readonly List<string> _imageFormats = new() { "jpeg", "webp", "avif" };

    private static readonly List<int> _imageSizes = new() { 25, 50, 100 };

    private static readonly List<ImageOptions> _imageOptions = new()
    {
        // Quality settings derived from webp values, using https://www.industrialempathy.com/posts/avif-webp-quality-settings/
        new AvifOptions(1, "_100", 50, 9),
        new AvifOptions(0.5, "_050", 50, 9),
        new AvifOptions(0.25, "_025", 55, 9),

        // Quality settings determined experimentally.
        new WebpOptions(1, "_100", 65, 6),
        new WebpOptions(0.5, "_050", 65, 6),
        new WebpOptions(0.25, "_025", 70, 6),

        // Quality settings derived from webp values, using https://www.industrialempathy.com/posts/avif-webp-quality-settings/
        new JpegOptions(1, "_100", 60),
        new JpegOptions(0.5, "_050", 60),
        new JpegOptions(0.25, "_025", 70),
    };

    private readonly StwDbContext _context;
    private readonly ILogger<ImageService> _logger;

    private readonly string _userPath;

    private readonly string _generatedPath;

    public ImageService(StwDbContext context, ILogger<ImageService> logger, string userPath, string generatedPath)
    {
        _userPath = userPath;
        _generatedPath = generatedPath;
        _context = context;
        _logger = logger;

        // NetVips caching doesn't really provide any benefit in this use case, no measurable performance impact
        // Turning it off reduces memory consumption somewhat
        Cache.MaxMem = 0;
    }

    public static List<ImageDto> CreateImageDtos(int id, DateTime? lastImageUpdate, List<int> imageSizes)
    {
        if (lastImageUpdate is null)
        {
            return new List<ImageDto>();
        }

        List<ImageDto> imageDtos = new();

        foreach (var imageFormat in _imageFormats)
        {
            foreach (var (pixelSize, percentSize) in imageSizes.Order().Zip(_imageSizes))
            {
                imageDtos.Add(new ImageDto($"{id}_{percentSize:D3}.{imageFormat}", pixelSize));
            }
        }

        return imageDtos;
    }

    public async void RunImageSetup()
    {
        await CreateDownScaledImages();
    }

    public async Task CreateDownScaledImages()
    {
        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        var engines = await _context.Engines.ToListAsync();

        foreach (var engine in engines)
        {
            await CreateDownscaledImageInternal(engine);
        }

        totalStopwatch.Stop();
        var elapsedSeconds = Math.Round(totalStopwatch.Elapsed.TotalSeconds);
        _logger.LogInformation("Updated images in {elapsedSeconds} seconds", elapsedSeconds);

        GC.Collect();
    }

    public async Task CreateDownscaledImage(Engine engine)
    {
        await CreateDownscaledImageInternal(engine);

        GC.Collect();
    }

    private async Task CreateDownscaledImageInternal(Engine engine)
    {
        var engineStopwatch = new Stopwatch();
        engineStopwatch.Start();

        var existingSourceImage = Directory.EnumerateFiles(_userPath, $"{engine.Id}.*").SingleOrDefault();
        var existingGeneratedImages = Directory.EnumerateFiles(_generatedPath, $"{engine.Id}_*.*");

        if (existingSourceImage == null)
        {
            if (existingGeneratedImages.Count() != _imageOptions.Count)
            {
                engine.ImageSizes.Clear();
                engine.LastImageUpdate = null;
                await _context.SaveChangesAsync();
            }

            return;
        }

        var sourceImageTimestamp = File.GetLastWriteTimeUtc(existingSourceImage);

        if (existingGeneratedImages.Count() == _imageOptions.Count && sourceImageTimestamp < engine.LastImageUpdate)
        {
            return;
        }

        ConcurrentBag<int> generatedPixelSizes = new();

        using var image = Image.NewFromFile(existingSourceImage);

        Parallel.ForEach(_imageOptions, options =>
        {
            // ReSharper disable once AccessToDisposedClosure - DownscaleImage shouldn't access the after the outer scope has disposed the image.
            var width = DownscaleImage(image, engine.Id, options);
            generatedPixelSizes.Add(width);
        });

        engine.ImageSizes = generatedPixelSizes.Distinct().ToList();

        engine.LastImageUpdate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        engineStopwatch.Stop();
        var elapsedSeconds = Math.Round(engineStopwatch.Elapsed.TotalSeconds);
        _logger.LogInformation("Updated images for {engine} in {elapsedSeconds}s", engine, elapsedSeconds);
    }

    private int DownscaleImage(Image image, int id, ImageOptions options)
    {
        var width = (int)Math.Round(image.Width * options.Scale);
        using var smallerImage = image.ThumbnailImage(width);

        var outputFileName = options.GetFilename(id);
        var outputFilePath = $"{_generatedPath}/{outputFileName}";
        switch (options)
        {
            case WebpOptions webpOptions:
                smallerImage.Webpsave(outputFilePath, webpOptions.Quality, effort: webpOptions.Effort, strip: true);
                break;

            case JpegOptions jpegOptions:
                smallerImage.Jpegsave(outputFilePath, jpegOptions.Quality, strip: true);
                break;

            case AvifOptions avifOptions:
                smallerImage.Heifsave(outputFilePath, avifOptions.Quality, effort: avifOptions.Effort, strip: true, compression: avifOptions.HeifCompression);
                break;
        }

        return smallerImage.Width;
    }
}