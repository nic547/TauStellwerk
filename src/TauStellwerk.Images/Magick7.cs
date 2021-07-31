// <copyright file="Magick7.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TauStellwerk.Util;

namespace TauStellwerk.Images
{
    public class Magick7 : MagickBase
    {
        public Magick7([NotNull] ICommandRunner runner)
            : base(runner)
        {
        }

        public override async Task<int> GetImageWidth(string path)
        {
            var (_, output) = await Runner.RunCommand("magick", $"identify -ping {path}");
            var match = SizeRegex.Match(output);
            return int.Parse(match.Groups["width"].Value);
        }

        public override async Task<bool> Resize(string input, string output, [Range(1, 99)] int outputScale, string additionalArguments = "")
        {
            var fileFormat = Path.GetExtension(output).Remove(0, 1);
            if (!SupportedFormats.Contains(fileFormat.ToUpperInvariant()))
            {
                return false;
            }

            var (returnCode, _) = await Runner.RunCommand("magick", $"{input} -resize {outputScale}% {additionalArguments} {output}");
            return returnCode == 0;
        }

        public override async Task<bool> IsAvailable()
        {
            try
            {
                var (exitCode, output) = await Runner.RunCommand("magick", "identify -list format");
                if (exitCode == 0)
                {
                    ConsoleService.PrintMessage("ImageMagick v3 seems to be available on this device.");

                    SupportedFormats.AddRange(
                        FormatRegex.Matches(output)
                            .Select(m => m.Groups["format"].Value)
                            .ToArray());

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
