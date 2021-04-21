// <copyright file="Magick6Tests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PiStellwerk.Images.Tests
{
    public class Magick6Tests
    {
        [Test]
        public async Task ExitCode1IsNotAvailable()
        {
            var runnerMock = new Mock<ICommandRunner>();
            runnerMock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1, string.Empty));
            var magick = new Magick6(runnerMock.Object);

            Assert.False(await magick.IsAvailable());
        }

        [Test]
        public async Task ExitCode0IsAvailable()
        {
            var runnerMock = new Mock<ICommandRunner>();
            runnerMock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((0, string.Empty));
            var magick = new Magick6(runnerMock.Object);

            Assert.True(await magick.IsAvailable());
        }
    }
}