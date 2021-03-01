// <copyright file="Magick3.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public class Magick3 : MagickBase
    {
        public override Task<int> GetImageWidth(string path)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsAvailable()
        {
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = "magick",
                        Arguments = "identify -version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                    },
                };
                p.Start();
                await p.WaitForExitAsync();
                if (p.ExitCode == 0)
                {
                    ConsoleService.PrintMessage("ImageMagick v3 seems to be available on this device.");
                    return true;
                }

                ConsoleService.PrintMessage("ImageMagick v3 not found on this device");
                return false;
            }
            catch (Exception)
            {
                ConsoleService.PrintMessage("ImageMagick v3 not found on this device");
                return false;
            }
        }
    }
}
