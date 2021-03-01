// <copyright file="MagickBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public abstract class MagickBase
    {
        private static MagickBase? _instance;

        internal Regex SizeRegex { get; } = new Regex(" (?<width>\\d+)x\\d+ ", RegexOptions.Compiled);

        public static async Task<MagickBase> GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var m3 = new Magick3();
            if (await m3.IsAvailable())
            {
                _instance = m3;
                return _instance;
            }

            var m2 = new Magick2();
            if (await m2.IsAvailable())
            {
                _instance = m2;
                return _instance;
            }

            ConsoleService.PrintWarning("No ImageMagick found on this system. Image related functionality won't be available!");
            _instance = new MagickNop();
            return _instance;
        }

        public abstract Task<bool> IsAvailable();

        public abstract Task<int> GetImageWidth(string path);

        internal async Task<(int ExitCode, string Output)> RunCommand(string command, string arguments)
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
            };
            p.Start();
            await p.WaitForExitAsync();
            var output = await p.StandardOutput.ReadToEndAsync();

            if (p.ExitCode != 0)
            {
                ConsoleService.PrintError("ImageMagick failed with \"" + await p.StandardError.ReadToEndAsync() + "\"");
            }

            return (p.ExitCode, output);
        }
    }
}
