// <copyright file="ImageSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetVips;
using TauStellwerk.Database;
using TauStellwerk.Database.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Images;

public class ImageSystem
{
    private readonly StwDbContext _context;

    private readonly string _userPath;

    private readonly string _generatedPath;

    private readonly List<ImageOptions> _imageOptions = new()
    {
        // Quality settings determined experimentally.
        new WebpOptions(1, "_100", 65, 6),
        new WebpOptions(0.5, "_050", 65, 6),
        new WebpOptions(0.25, "_025", 70, 6),

        // Quality settings derived from webp values, using https://www.industrialempathy.com/posts/avif-webp-quality-settings/
        new JpegOptions(1, "_100", 60),
        new JpegOptions(0.5, "_050", 60),
        new JpegOptions(0.25, "_025", 70),
        new AvifOptions(1, "_100", 50, 6),
        new AvifOptions(0.5, "_050", 50, 6),
        new AvifOptions(0.25, "_025", 55, 6),
    };

    public ImageSystem(StwDbContext context, string userPath, string generatedPath)
    {
        _userPath = userPath;
        _generatedPath = generatedPath;
        _context = context;
    }

    public async void RunImageSetup()
    {
        await CreateDownScaledImages();
    }

    private async Task CreateDownScaledImages()
    {
        foreach (var engine in _context.Engines.Include(e => e.Images))
        {
            var file = Directory.EnumerateFiles(_userPath, $"{engine.Id}.*").SingleOrDefault();

            if (file == null)
            {
                continue;
            }

            var sourceImageTimestamp = File.GetLastWriteTime(file);

            var updatedImages = 0;

            foreach (var options in _imageOptions)
            {
                var filename = $"{engine.Id}{options.Suffix}{options.FileEnding}";
                var existingImage = Directory.EnumerateFiles(_generatedPath, options.GetFilename(engine.Id)).SingleOrDefault();
                DateTime existingImageTimestamp = DateTime.MaxValue;

                if (existingImage != null)
                {
                    existingImageTimestamp = File.GetLastWriteTime(existingImage);
                }

                if (engine.LastImageUpdate == null || existingImageTimestamp > engine.LastImageUpdate || sourceImageTimestamp > engine.LastImageUpdate)
                {
                    var width = await DownscaleImage(file, engine.Id, options);
                    updatedImages++;

                    var existingEntry = engine.Images.SingleOrDefault(i => i.Filename == options.GetFilename(engine.Id));
                    if (existingEntry is null)
                    {
                        engine.Images.Add(new EngineImage
                        {
                            Filename = filename,
                            Width = width,
                        });
                    }
                    else
                    {
                        existingEntry.Width = width;
                    }
                }
            }

            if (updatedImages != 0)
            {
                engine.LastImageUpdate = DateTime.Now;
                await _context.SaveChangesAsync();
                ConsoleService.PrintHighlightedMessage($"Updated {updatedImages} images for '{engine}'");
            }
        }
    }

    private Task<int> DownscaleImage(string inputFilePath, int id, ImageOptions options)
    {
        using var image = Image.NewFromFile(inputFilePath);
        using var smallerImage = image.Resize(options.Scale);

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

        return Task.FromResult(smallerImage.Width);
    }
}