// <copyright file="ImageDirectoryAnalyzer.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;
using PiStellwerk.Data.Model;

namespace PiStellwerk.Images
{
    public class ImageDirectoryAnalyzer
    {
        private StwDbContext _context;
        private string _userPath;

        public ImageDirectoryAnalyzer(StwDbContext context, string userPath)
        {
            _userPath = userPath;
            _context = context;
        }

        public async void CheckAndGenerateImages()
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
