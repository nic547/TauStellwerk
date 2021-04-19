// <copyright file="ImageSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;
using PiStellwerk.Data.Model;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public class ImageSystem
    {
        private readonly (string Prefix, int Size)[] _downScaleValues = new[] { ("half_", 50), ("quarter_", 25) };

        private readonly StwDbContext _context;
        private readonly string _userPath;
        private readonly string _generatedPath;

        public ImageSystem(StwDbContext context, string userPath, string generatedPath)
        {
            _userPath = userPath;
            _generatedPath = generatedPath;
            _context = context;
        }

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

            await CheckForLostUserFiles();
            await CheckForMissingImageWidth();
            await CreateDownScaledImages();
        }

        private async Task CheckForLostUserFiles()
        {
            var files = Directory.GetFiles(_userPath);
            foreach (string file in files)
            {
                var existingEntry = await _context.EngineImages.SingleOrDefaultAsync(x => x.Filename == Path.GetFileName(file));

                if (existingEntry != null)
                {
                    continue;
                }

                var success = int.TryParse(Path.GetFileNameWithoutExtension(file), out var idCandidate);

                if (!success)
                {
                    continue;
                }

                var engine = await _context.Engines.SingleOrDefaultAsync(e => e.Id == idCandidate);

                engine?.Image.Add(new EngineImage(Path.GetFileName(file))
                {
                    IsGenerated = false,
                });

                await _context.SaveChangesAsync();
            }
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
            foreach (var engine in _context.Engines.Include(e => e.Image))
            {
                if (!engine.Image.Any() || engine.Image.Count == _downScaleValues.Length + 1)
                {
                    continue;
                }

                var originalImage = engine.Image.SingleOrDefault(i => !i.IsGenerated);
                if (originalImage == null)
                {
                    continue;
                }

                foreach (var (prefix, size) in _downScaleValues)
                {
                    var newImage = await DownscaleImage(Path.Combine(_userPath, originalImage.Filename), prefix, size);
                    if (newImage != null)
                    {
                        engine.Image.Add(
                            new EngineImage(newImage.Value.Filename)
                            {
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
            var newFileName = prefix + Path.GetFileName(inputFilePath);
            var newFilePath = Path.Combine(_generatedPath, newFileName);
            var magick = await MagickBase.GetInstance();

            var success = await magick.Resize(inputFilePath, newFilePath, size);
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
