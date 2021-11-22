// <copyright file="Magick7Tests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace TauStellwerk.Images.Tests;

public class Magick7Tests
{
    private const string FormatResponse = "    Format  Module    Mode  Description\r\n-------------------------------------------------------------------------------\r\n      BMP* BMP       rw-   Microsoft Windows bitmap image\r\n      GIF* GIF       rw+   CompuServe graphics interchange format\r\n      JPE* JPEG      rw-   Joint Photographic Experts Group JFIF format (libjpeg-turbo 2.0.5)\r\n     JPEG* JPEG      rw-   Joint Photographic Experts Group JFIF format (libjpeg-turbo 2.0.5)\r\n      JPG* JPEG      rw-   Joint Photographic Experts Group JFIF format (libjpeg-turbo 2.0.5)\r\n      PNG* PNG       rw-   Portable Network Graphics (libpng 1.6.37)\r\n\r\n* native blob support\r\nr read support\r\nw write support\r\n+ support for multiple images";

    [Test]
    public async Task ExitCode1IsNotAvailable()
    {
        var runnerMock = new Mock<ICommandRunner>();
        runnerMock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((1, string.Empty));
        var magick = new Magick7(runnerMock.Object);

        Assert.False(await magick.IsAvailable());
    }

    [Test]
    public async Task ExitCode0IsAvailable()
    {
        var runnerMock = new Mock<ICommandRunner>();
        runnerMock.Setup(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((0, string.Empty));
        var magick = new Magick7(runnerMock.Object);

        Assert.True(await magick.IsAvailable());
    }

    [Test]
    public async Task CanParseWidthCorrectly()
    {
        var runnerMock = new Mock<ICommandRunner>();
        runnerMock.Setup(m => m.RunCommand("magick", It.IsAny<string>())).ReturnsAsync((0, "image.webp WEBP 1920x960 1920x960+0+0 8-bit sRGB 98858B 0.016u 0:00.014"));
        var magick = new Magick7(runnerMock.Object);

        var result = await magick.GetImageWidth("image.webp");
        result.Should().Be(1920);
    }

    [Test]
    public async Task CanOnlyResizeToSupportedFormats()
    {
        var runnerMock = new Mock<ICommandRunner>();
        runnerMock.Setup(m => m.RunCommand("magick", It.IsAny<string>())).ReturnsAsync((0, string.Empty));
        runnerMock.Setup(m => m.RunCommand("magick", "identify -list format")).ReturnsAsync((0, FormatResponse));
        var magick = new Magick7(runnerMock.Object);

        (await magick.IsAvailable()).Should().BeTrue();

        (await magick.Resize("image.png", "image.webp", 100)).Should().BeFalse();
        (await magick.Resize("image.png", "image.avif", 100)).Should().BeFalse();
        (await magick.Resize("image.png", "image.jpg", 100)).Should().BeTrue();
        (await magick.Resize("image.png", "image.bmp", 100)).Should().BeTrue();
    }

    [Test]
    public async Task CanSupplyAdditionalArguments()
    {
        var runnerMock = new Mock<ICommandRunner>();
        var arguments = string.Empty;

        runnerMock.Setup(m => m.RunCommand("magick", It.IsAny<string>()))
            .ReturnsAsync((0, string.Empty))
            .Callback<string, string>((_, args) => { arguments = args; });
        runnerMock.Setup(m => m.RunCommand("magick", "identify -list format")).ReturnsAsync((0, FormatResponse));

        var magick = new Magick7(runnerMock.Object);

        _ = await magick.IsAvailable();
        _ = await magick.Resize("image.png", "image.jpg", 100, "testString");

        arguments.Should().Contain("testString");
    }
}
