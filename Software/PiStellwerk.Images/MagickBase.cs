// <copyright file="MagickBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace PiStellwerk.Images
{
    public abstract class MagickBase
    {
        public abstract Task<bool> IsAvailable();

        public abstract Task<int> GetImageWidth(string path);
    }
}
