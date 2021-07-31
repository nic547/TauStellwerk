// <copyright file="ImageSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Database;
using TauStellwerk.Database.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Images
{
    public class ImageSystem
    {
        private readonly (string Prefix, int Size)[] _downScaleValues = { ("full_", 100), ("half_", 50), ("quarter_", 25) };

        private readonly StwDbContext _context;

        private readonly string _userPath;

        private readonly string _generatedPath;

        public ImageSystem(StwDbContext context, string userPath, string generatedPath)
        {
            _userPath = userPath;
            _generatedPath = generatedPath;
            _context = context;
        }

        /// <summary>
        /// Gets list containing additional arguments that should be passed to IM, per format and per Size.
        /// </summary>
        private List<(string Format, int Scaling, string Arguments)> AdditionalArguments { get; } = new()
        {
            ("WEBP", 025, "-quality 70 -define webp:image-hint=photo -define webp:method=6 -define webp:auto-filter=true -strip"),
            ("WEBP", 050, "-quality 65 -define webp:image-hint=photo -define webp:method=6 -define webp:auto-filter=true -strip"),
            ("WEBP", 100, "-quality 65 -define webp:image-hint=photo -define webp:method=6 -define webp:auto-filter=true -strip"),
        };

        public static async Task<bool> HasMagickAvailable()
        {
            var magick = await MagickBase.GetInstance();
            if (magick.GetType() == typeof(MagickNop))
            {
                return false;
            }

            return true;
        }

        public async void RunImageSetup()
        {
            if (!await HasMagickAvailable())
            {
                return;
            }

            await CheckForMissingImageWidth();
            await CreateDownScaledImages();
        }

        private async Task CheckForMissingImageWidth()
        {
            var magick = await MagickBase.GetInstance();
            var imagesMissingWidth = await _context.EngineImages.Where(i => i.Width == 0).ToListAsync();

            foreach (var image in imagesMissingWidth)
            {
                string path = image.IsGenerated ? _generatedPath : _userPath;
                var width = await magick.GetImageWidth(Path.Combine(path, image.Filename));
                if (width != 0)
                {
                    image.Width = width;
                }
                else
                {
                    ConsoleService.PrintWarning($"Image {image.Filename} has no width and width couldn't be determined.");
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateDownScaledImages()
        {
            foreach (var engine in _context.Engines.Include(e => e.Images))
            {
                if (engine.Images.Count == _downScaleValues.Length)
                {
                    continue;
                }

                var file = Directory.EnumerateFiles(_userPath, $"{engine.Id}.*").SingleOrDefault();
                if (file == null)
                {
                    continue;
                }

                foreach (var (prefix, size) in _downScaleValues)
                {
                    var newImage = await DownscaleImage(Path.Combine(_userPath, file), prefix, size);
                    if (newImage != null)
                    {
                        engine.Images.Add(
                            new EngineImage
                            {
                                Filename = newImage.Value.Filename,
                                IsGenerated = true,
                                Width = newImage.Value.Width,
                            });
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task<(string Filename, int Width)?> DownscaleImage(string inputFilePath, string prefix, int size)
        {
            var magick = await MagickBase.GetInstance();

            var newFileName = prefix + Path.GetFileName(inputFilePath);
            var newFilePath = Path.Combine(_generatedPath, newFileName);
            var format = Path.GetExtension(newFileName).Remove(0, 1).ToUpperInvariant();

            var additionalArguments = AdditionalArguments.SingleOrDefault(t => t.Format == format && t.Scaling == size);
            var success = await magick.Resize(inputFilePath, newFilePath, size, additionalArguments.Arguments);
            if (!success)
            {
                ConsoleService.PrintError($"Failed to downscale {Path.GetFileName(inputFilePath)}");
                return null;
            }

            var width = await magick.GetImageWidth(newFilePath);
            if (width == 0)
            {
                ConsoleService.PrintError($"{newFileName}'s size could not be determined.");
            }

            return (newFileName, width);
        }
    }
}
