// <copyright file="Magick2.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public class Magick2 : MagickBase
    {
        public override async Task<bool> IsAvailable()
        {
            try
            {
                var (exitCode, _) = await RunCommand("identify", "-version");
                if (exitCode == 0)
                {
                    ConsoleService.PrintMessage("ImageMagick v2 seems to be available on this device.");
                    return true;
                }

                ConsoleService.PrintMessage("ImageMagick v2 not found on this device");
                return false;
            }
            catch (Exception)
            {
                ConsoleService.PrintMessage("ImageMagick v2 not found on this device");
                return false;
            }
        }

        public override async Task<int> GetImageWidth(string path)
        {
            var (_, output) = await RunCommand("identify", $"-ping {path}");
            var match = SizeRegex.Match(output);
            return int.Parse(match.Groups["width"].Value);
        }
    }
}
