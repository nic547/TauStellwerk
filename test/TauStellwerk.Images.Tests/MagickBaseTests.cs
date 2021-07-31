// <copyright file="MagickBaseTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace TauStellwerk.Images.Tests
{
    public class MagickBaseTests
    {
        [SetUp]
        public void SetUp()
        {
            MagickBase.ClearInstance();
        }

        [TearDown]
        public void TearDown()
        {
            MagickBase.ClearInstance();
        }

        [Test]
        public async Task ReturnsNopIfNoneAvailable()
        {
            var mock = new Mock<ICommandRunner>();
            mock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1, " "));

            var magick = await MagickBase.GetInstance(mock.Object);
            magick.Should().BeAssignableTo<MagickNop>();
        }

        [Test]
        public async Task ReturnsMagick6()
        {
            var mock = new Mock<ICommandRunner>();
            mock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1, " "));
            mock.Setup(m => m.RunCommand("identify", It.IsAny<string>())).ReturnsAsync((0, " "));

            var magick = await MagickBase.GetInstance(mock.Object);
            magick.Should().BeAssignableTo<Magick6>();
        }

        [Test]
        public async Task ReturnsMagick7()
        {
            var mock = new Mock<ICommandRunner>();
            mock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1, " "));
            mock.Setup(m => m.RunCommand("magick", It.IsAny<string>())).ReturnsAsync((0, " "));

            var magick = await MagickBase.GetInstance(mock.Object);
            magick.Should().BeAssignableTo<Magick7>();
        }
    }
}
