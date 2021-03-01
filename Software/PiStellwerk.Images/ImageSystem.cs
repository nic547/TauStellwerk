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
        }

        private async Task CheckForLostUserFiles()
        {
            var files = Directory.GetFiles(_userPath);
            foreach (string file in files)
            {
                var existingEntry = await _context.Engineimages.SingleOrDefaultAsync(x => x.Filename == Path.GetFileName(file));

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
            var imagesMissingWidth = await _context.Engineimages.Where(i => i.Width == 0).ToListAsync();

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
    }
}
