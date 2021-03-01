// <copyright file="ImageSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;
using PiStellwerk.Data.Model;

namespace PiStellwerk.Images
{
    public class ImageSystem
    {
        private readonly StwDbContext _context;
        private readonly string _userPath;

        public ImageSystem(StwDbContext context, string userPath)
        {
            _userPath = userPath;
            _context = context;
        }

        public async void RunImageSetup()
        {
            await CheckForLostUserFiles();

            _ = await MagickFactory.Create();
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

                engine?.Image.Add(new EngineImage()
                {
                    Filename = Path.GetFileName(file),
                    IsGenerated = false,
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
