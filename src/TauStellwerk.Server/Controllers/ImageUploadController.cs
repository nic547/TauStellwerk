// <copyright file="ImageUploadController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TauStellwerk.Data;
using TauStellwerk.Data.ImageService;

namespace TauStellwerk.Server.Controllers;

[ApiController]
public class ImageUploadController : ControllerBase
{
    private readonly TauStellwerkOptions _options;
    private readonly ImageService _imageService;
    private readonly StwDbContext _context;

    public ImageUploadController(IOptions<TauStellwerkOptions> options, ImageService imageService, StwDbContext context)
    {
        _options = options.Value;
        _imageService = imageService;
        _context = context;
    }

    [HttpPost]
    [Route("/upload/{id:int}")]
    public async Task UploadImage(int id, IFormFile image)
    {
        var engine = await _context.Engines.SingleOrDefaultAsync(e => e.Id == id);

        if (engine is null)
        {
            return;
        }

        TryDeleteBackup(id);
        TryBackupCurrentImage(id);

        var newFileExtension = Path.GetExtension(image.FileName);
        await using (var newFile = System.IO.File.Create(Path.Combine(_options.OriginalImageDirectory, id + newFileExtension)))
        {
            await image.CopyToAsync(newFile, CancellationToken.None);
            await newFile.FlushAsync();
        }

        _ = _imageService.CreateDownscaledImage(engine);
    }

    private void TryDeleteBackup(int id)
    {
        var backupImage = Directory.EnumerateFiles(_options.OriginalImageDirectory, $"{id}_old.*").SingleOrDefault();

        if (backupImage is not null)
        {
            System.IO.File.Delete(backupImage);
        }
    }

    private void TryBackupCurrentImage(int id)
    {
        var currentImage = Directory.EnumerateFiles(_options.OriginalImageDirectory, $"{id}.*").SingleOrDefault();

        if (currentImage is not null)
        {
            var extension = Path.GetExtension(currentImage);
            var path = Path.GetDirectoryName(currentImage);
            System.IO.File.Move(currentImage, $"{path}{Path.DirectorySeparatorChar}{id}_old{extension}");
        }
    }
}
