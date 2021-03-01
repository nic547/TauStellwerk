// <copyright file="MagickFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
{
    public static class MagickFactory
    {
        public static async Task<MagickBase?> Create()
        {
            var m3 = new Magick3();
            if (await m3.IsAvailable())
            {
                return m3;
            }

            var m2 = new Magick2();
            if (await m2.IsAvailable())
            {
                return m2;
            }

            ConsoleService.PrintWarning("No ImageMagick found on this system. Image related functionality won't be available!");
            return null;
        }
    }
}
