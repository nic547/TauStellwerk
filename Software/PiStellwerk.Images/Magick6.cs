// <copyright file="Magick6.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public class Magick6 : MagickBase
    {
        public Magick6(ICommandRunner runner)
            : base(runner)
        {
        }

        public override async Task<bool> IsAvailable()
        {
            try
            {
                var (exitCode, output) = await Runner.RunCommand("identify", "-list format");
                if (exitCode == 0)
                {
                    ConsoleService.PrintMessage("ImageMagick v2 seems to be available on this device.");

                    SupportedFormats.AddRange(
                        FormatRegex.Matches(output)
                            .Select(m => m.Groups["format"].Value)
                            .ToArray());

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
            var (_, output) = await Runner.RunCommand("identify", $"-ping {path}");
            var match = SizeRegex.Match(output);
            return int.Parse(match.Groups["width"].Value);
        }

        public override async Task<bool> Resize(string input, string output, int outputScale, string additionalArguments = "")
        {
            var fileFormat = Path.GetExtension(output).Remove(0, 1);
            if (!SupportedFormats.Contains(fileFormat.ToUpperInvariant()))
            {
                return false;
            }

            var (returnCode, _) = await Runner.RunCommand("convert", $"{input} -resize {outputScale}% {additionalArguments} {output}");
            return returnCode == 0;
        }
    }
}
