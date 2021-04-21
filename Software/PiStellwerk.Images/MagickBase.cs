// <copyright file="MagickBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public abstract class MagickBase
    {
        private static MagickBase? _instance;

        protected MagickBase(ICommandRunner runner)
        {
            Runner = runner;
        }

        internal ICommandRunner Runner { get; }

        protected Regex SizeRegex { get; } = new Regex(" (?<width>\\d+)x\\d+ ", RegexOptions.Compiled);

        public static async Task<MagickBase> GetInstance()
        {
            return await GetInstance(new CommandRunner());
        }

        public static async Task<MagickBase> GetInstance(ICommandRunner runner)
        {
            if (_instance != null)
            {
                return _instance;
            }

            var m3 = new Magick3(runner);
            if (await m3.IsAvailable())
            {
                _instance = m3;
                return _instance;
            }

            var m2 = new Magick2(runner);
            if (await m2.IsAvailable())
            {
                _instance = m2;
                return _instance;
            }

            ConsoleService.PrintWarning("No ImageMagick found on this system. Image related functionality won't be available!");
            _instance = new MagickNop(runner);
            return _instance;
        }

        public abstract Task<bool> IsAvailable();

        public abstract Task<int> GetImageWidth(string path);

        public abstract Task<bool> Resize(string input, string output, [Range(1, 99)] int outputScale);
    }
}
