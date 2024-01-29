// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetVips;
using TauStellwerk.Base.Dto;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data.ImageService;

public class ImageService
{
    private static readonly List<string> _imageFormats = ["jpeg", "webp", "avif"];

    private static readonly List<int> _imageSizes = [25, 50, 100];

    private static readonly List<ImageOptions> _imageOptions =
    [
        // Quality settings derived from webp values, using https://www.industrialempathy.com/posts/avif-webp-quality-settings/
        new AvifOptions(1, "_100", 50, 8),
        new AvifOptions(0.5, "_050", 50, 8),
        new AvifOptions(0.25, "_025", 55, 8),

        // Quality settings determined experimentally.
        //new WebpOptions(1, "_100", 65, 6),
        //new WebpOptions(0.5, "_050", 65, 6),
        //new WebpOptions(0.25, "_025", 70, 6),

        // Quality settings derived from webp values, using https://www.industrialempathy.com/posts/avif-webp-quality-settings/
        new JpegOptions(1, "_100", 60),
        new JpegOptions(0.5, "_050", 60),
        new JpegOptions(0.25, "_025", 70),
    ];

    private readonly StwDbContext _context;

    private readonly SemaphoreSlim _contextSemaphore = new(1);
    private int _TotalEngineCount;

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
            return [];
        }

        List<ImageDto> imageDtos = [];

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

        await Parallel.ForEachAsync(engines, async (engine, _) =>
        {
            await CreateDownscaledImageInternal(engine);
        });

        totalStopwatch.Stop();
        var elapsedSeconds = Math.Round(totalStopwatch.Elapsed.TotalSeconds);
        _logger.LogInformation("Updated {totalImages} images in {elapsedSeconds} seconds", _TotalEngineCount, elapsedSeconds);
        _TotalEngineCount = 0;

        GC.Collect();
    }

    public async Task CreateDownscaledImage(Engine engine)
    {
        await CreateDownscaledImageInternal(engine);

        GC.Collect();
    }

    private async Task CreateDownscaledImageInternal(Engine engine)
    {
        var existingSourceImage = Directory.EnumerateFiles(_userPath, $"{engine.Id}.*").SingleOrDefault();
        var existingGeneratedImages = Directory.EnumerateFiles(_generatedPath, $"{engine.Id}_*.*");

        if (existingSourceImage == null)
        {
            if (existingGeneratedImages.Count() != _imageOptions.Count)
            {
                engine.ImageSizes.Clear();
                engine.LastImageUpdate = null;
                await _contextSemaphore.WaitAsync();
                await _context.SaveChangesAsync();
                _contextSemaphore.Release();
            }

            return;
        }

        var sourceImageTimestamp = File.GetLastWriteTimeUtc(existingSourceImage);

        if (existingGeneratedImages.Count() == _imageOptions.Count && sourceImageTimestamp < engine.LastImageUpdate)
        {
            return;
        }

        ConcurrentBag<int> generatedPixelSizes = [];

        using var image = Image.NewFromFile(existingSourceImage);

        Parallel.ForEach(_imageOptions, options =>
        {
            // ReSharper disable once AccessToDisposedClosure - DownscaleImage shouldn't access the after the outer scope has disposed the image.
            var width = DownscaleImage(image, engine.Id, options);
            generatedPixelSizes.Add(width);
        });

        engine.ImageSizes = generatedPixelSizes.Distinct().ToList();

        engine.LastImageUpdate = DateTime.UtcNow;

        await _contextSemaphore.WaitAsync();
        await _context.SaveChangesAsync();
        _contextSemaphore.Release();

        _logger.LogInformation("Updated images for {engine}", engine);
        Interlocked.Increment(ref _TotalEngineCount);
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
                smallerImage.Webpsave(outputFilePath, webpOptions.Quality, effort: webpOptions.Effort);
                break;

            case JpegOptions jpegOptions:
                smallerImage.Jpegsave(outputFilePath, jpegOptions.Quality);
                break;

            case AvifOptions avifOptions:
                smallerImage.Heifsave(outputFilePath, avifOptions.Quality, effort: avifOptions.Effort, compression: avifOptions.HeifCompression);
                break;
            default:
                throw new InvalidOperationException("Unknown imageOption subtype");
        }

        return smallerImage.Width;
    }
}
