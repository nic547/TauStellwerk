// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using CommandLine;
using PiStellwerk.Tools.LoadTest;

namespace PiStellwerk.Tools
{
    /// <summary>
    /// A tool that tries to simulate a "realistic user" by making requests to the PiStellwerk-server.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point for the console application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<LoadTestOptions>(args).WithParsedAsync(async options => await LoadTester.Run(options));
        }
    }
}
