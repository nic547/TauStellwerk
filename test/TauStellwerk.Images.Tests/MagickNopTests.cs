// <copyright file="MagickNopTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Images.Tests
{
    public class MagickNopTests
    {
        [Test]
        public async Task IsAlwaysAvailable()
        {
            var commandRunner = new ReportingCommandRunner();
            MagickBase magick = new MagickNop(commandRunner);

            (await magick.IsAvailable()).Should().BeTrue();
            commandRunner.HasBeenUsed.Should().BeFalse();
        }

        [Test]
        public async Task ResizeDoesNothing()
        {
            var commandRunner = new ReportingCommandRunner();
            MagickBase magick = new MagickNop(commandRunner);

            _ = await magick.Resize("input.png", "output.webp", 100);
            commandRunner.HasBeenUsed.Should().BeFalse();
        }

        [Test]
        public async Task GetSizeDoesNothing()
        {
            var commandRunner = new ReportingCommandRunner();
            MagickBase magick = new MagickNop(commandRunner);

            _ = await magick.GetImageWidth("image.png");
            commandRunner.HasBeenUsed.Should().BeFalse();
        }

        [Test]
        public async Task UsingCommandRunnerGetsReported()
        {
            var commandRunner = new ReportingCommandRunner();
            _ = await commandRunner.RunCommand("ijdfihjdifji", "dikjfiusdfiousbdfujisbndifuhnsduiofn");

            commandRunner.HasBeenUsed.Should().BeTrue();
        }

        private class ReportingCommandRunner : ICommandRunner
        {
            public bool HasBeenUsed { get; private set; }

            public Task<(int ExitCode, string Output)> RunCommand(string command, string arguments)
            {
                HasBeenUsed = true;
                return Task.FromResult((0, string.Empty));
            }
        }
    }
}