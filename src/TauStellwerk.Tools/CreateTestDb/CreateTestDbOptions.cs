// <copyright file="CreateTestDbOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using CommandLine;

namespace TauStellwerk.Tools.CreateTestDb;

[Verb("create-test-db", HelpText = "Creates a datbase with random test data")]
public record CreateTestDbOptions
{
    [Option('f', "filename", Required = true, HelpText = "The filename of the database file that is created.")]
    public string Filename { get; init; } = string.Empty;

    [Option('c', "count", Required = false, Default = 100, HelpText = "How many engines should be generated.")]
    public int Count { get; init; }
}
