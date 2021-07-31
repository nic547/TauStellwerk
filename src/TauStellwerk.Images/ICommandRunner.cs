// <copyright file="ICommandRunner.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TauStellwerk.Util;

namespace TauStellwerk.Images
{
    public interface ICommandRunner
    {
        public Task<(int ExitCode, string Output)> RunCommand(string command, string arguments);
    }

    public class CommandRunner : ICommandRunner
    {
        public async Task<(int ExitCode, string Output)> RunCommand(string command, string arguments)
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
            string output = string.Empty;

            try
            {
                p.Start();
                output = await p.StandardOutput.ReadToEndAsync();
                await p.WaitForExitAsync();
            }
            catch (Exception)
            {
                // ignored
            }

            if (p.ExitCode != 0)
            {
                ConsoleService.PrintError("ImageMagick failed with \"" + await p.StandardError.ReadToEndAsync() + "\"");
            }

            return (p.ExitCode, output);
        }
    }
}