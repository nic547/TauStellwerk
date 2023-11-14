// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommandLine;
using TauStellwerk.Tools.CreateTestDb;
using TauStellwerk.Tools.LoadTest;

namespace TauStellwerk.Tools;

/// <summary>
/// A tool that tries to simulate a "realistic user" by making requests to the TauStellwerk-server.
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
        var parsedArguments = Parser.Default.ParseArguments<LoadTestOptions, CreateTestDbOptions>(args);

        await parsedArguments.WithParsedAsync<LoadTestOptions>(LoadTester.Run);
        await parsedArguments.WithParsedAsync<CreateTestDbOptions>(CreateTestDbTool.Run);
    }
}
