// <copyright file="EngineImageControl.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using FluentResults;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.Controls;

[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "This specific class looks better this way.")]
public class EngineImageControl : Image
{
    public static readonly DirectProperty<EngineImageControl, IImmutableList<EngineImage>?> EngineImagesProperty =
        AvaloniaProperty.RegisterDirect<EngineImageControl, IImmutableList<EngineImage>?>(
            nameof(EngineImages),
            i => i.EngineImages,
            (o, v) => o.EngineImages = v);

    public static readonly DirectProperty<EngineImageControl, int> ImageTimestampProperty =
        AvaloniaProperty.RegisterDirect<EngineImageControl, int>(
            nameof(ImageTimestamp),
            i => i.ImageTimestamp,
            (o, v) => o.ImageTimestamp = v);

    private const string ImageUrlFragment = "images/";

    // I don't want to write to a temp directory, because having the image cache persist is a positive side effect.
    // Saving them in the program directory isn't perfect, but at least it's easy to delete the software and associated data.
    private const string ImageCacheLocation = "./cache/images/";
    private const string DefaultImagePath = "avares://TauStellwerk.Desktop/Assets/noImageImage.png";

    private static Bitmap? _defaultImage;

    private static HttpClient? _httpClient;

    private IImmutableList<EngineImage>? _engineImages;
    private int _imageTimestamp;

    public IImmutableList<EngineImage>? EngineImages
    {
        get => _engineImages;
        set
        {
            SetAndRaise(EngineImagesProperty, ref _engineImages, value);
            _ = Task.Run(() => LoadImages(value)).ConfigureAwait(false);
        }
    }

    public int ImageTimestamp
    {
        get => _imageTimestamp;
        set => SetAndRaise(ImageTimestampProperty, ref _imageTimestamp, value);
    }

    protected override void OnInitialized()
    {
        if (_defaultImage is null)
        {
            _defaultImage = new Bitmap(AssetLoader.Open(new Uri(DefaultImagePath)));
        }

        Source = _defaultImage;
        base.OnInitialized();
    }

    private async Task LoadImages(IImmutableList<EngineImage>? images)
    {
        if (images is null)
        {
            return;
        }

        var imagePathResult = await DownloadImageToFilesystem(images, ImageTimestamp);
        if (imagePathResult.IsFailed)
        {
            throw new Exception("Failed to download image");
        }

        await Dispatcher.UIThread.InvokeAsync(() => { Source = new Bitmap(imagePathResult.Value); });
    }

    private static async Task<Result<string>> DownloadImageToFilesystem(IImmutableList<EngineImage> images, int timeStamp)
    {
        var bestCandidate = SelectBestImageCandidate(images);

        if (bestCandidate is null)
        {
            return Result.Fail("No suitable images were provided.");
        }

        var fileNameLocation = Path.Combine(ImageCacheLocation, FileNameWithTimestamp(bestCandidate.Filename, timeStamp));

        if (File.Exists(fileNameLocation))
        {
            return fileNameLocation;
        }

        var client = await GetHttpClient();

        try
        {
            var imageBytes = await client.GetByteArrayAsync(ImageUrlFragment + bestCandidate.Filename);

            Directory.CreateDirectory(ImageCacheLocation);

            await File.WriteAllBytesAsync(fileNameLocation, imageBytes);

            return Result.Ok(fileNameLocation);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private static EngineImage? SelectBestImageCandidate(IImmutableList<EngineImage> image)
    {
        /* For now a very basic heuristic is used.
        Webp seems to be supported on all platforms
        Loading large images directly tanks performance. From testing it seems that staying below 1000px width works.
        AFAIK there's a way to scale down images before loading them as bitmaps to lower the performance impact, but that's for future me.
        */

        return image.Where(i => i.Type == "image/webp" && i.Width < 1000).MaxBy(i => i.Width);
    }

    private static string FileNameWithTimestamp(string filename, int timestamp)
    {
        var segements = filename.Split('.');
        return $"{segements[0]}_{timestamp:X}.{segements[1]}";
    }

    private static async Task<HttpClient> GetHttpClient()
    {
        if (_httpClient is not null)
        {
            return _httpClient;
        }

        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };

        var settingService = Locator.Current.GetService<ISettingsService>() ??
                             throw new InvalidOperationException("Settings service is not available.");
        var settings = await settingService.GetSettings();

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(settings.ServerAddress),
        };

        return _httpClient;
    }
}