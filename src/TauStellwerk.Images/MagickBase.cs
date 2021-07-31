// <copyright file="MagickBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TauStellwerk.Util;

namespace TauStellwerk.Images
{
    public abstract class MagickBase
    {
        private static MagickBase? _instance;

        protected MagickBase(ICommandRunner runner)
        {
            Runner = runner;
        }

        internal ICommandRunner Runner { get; }

        internal List<string> SupportedFormats { get; } = new();

        protected Regex SizeRegex { get; } = new(" (?<width>\\d+)x\\d+ ", RegexOptions.Compiled);

        protected Regex FormatRegex { get; } = new(
            "^ *(?<format>\\w+)(\\*| ) .*(r|-)(w|-)(\\+|-).*$",
            RegexOptions.Multiline | RegexOptions.Compiled);

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

            var m7 = new Magick7(runner);
            if (await m7.IsAvailable())
            {
                _instance = m7;
                return _instance;
            }

            var m6 = new Magick6(runner);
            if (await m6.IsAvailable())
            {
                _instance = m6;
                return _instance;
            }

            ConsoleService.PrintWarning("No ImageMagick found on this system. Image related functionality won't be available!");
            _instance = new MagickNop(runner);
            return _instance;
        }

        /// <summary>
        /// Clear the instance returned. Subsequent request will return a different instance.
        /// </summary>
        public static void ClearInstance()
        {
            _instance = null;
        }

        /// <summary>
        /// Check if a given adapter is available on a device and check what file formats are supported.
        /// </summary>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task<bool> IsAvailable();

        public abstract Task<int> GetImageWidth(string path);

        public abstract Task<bool> Resize(string input, string output, [Range(1, 99)] int outputScale, string additionalArguments = "");
    }
}
