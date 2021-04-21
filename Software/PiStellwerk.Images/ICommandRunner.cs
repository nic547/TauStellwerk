// <copyright file="ICommandRunner.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Images
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