// <copyright file="MagickNop.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace TauStellwerk.Images;

/// <summary>
/// ImageMagick class that does nothing. Used if ImageMagick isn't installed.
/// </summary>
public class MagickNop : MagickBase
{
    public MagickNop(ICommandRunner runner)
        : base(runner)
    {
    }

    public override Task<int> GetImageWidth(string path)
    {
        return Task.FromResult(0);
    }

    public override Task<bool> Resize(string input, string output, int outputScale, string additionalArguments = "")
    {
        return Task.FromResult(false);
    }

    public override Task<bool> IsAvailable()
    {
        return Task.FromResult(true);
    }
}