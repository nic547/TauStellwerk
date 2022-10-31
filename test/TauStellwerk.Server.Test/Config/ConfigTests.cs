// <copyright file="ConfigTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TauStellwerk.Server;
using TauStellwerk.Server.CommandStations;

namespace TauStellwerk.Test.Config;
public class ConfigTests
{
    [Test]
    public void TestTestConfig()
    {
        ConfigurationBuilder builder = new();
        builder.AddJsonFile("./Config/testConfig.json");
        var config = builder.Build();

        var options = config.Get<TauStellwerkOptions>();

        options.OriginalImageDirectory.Should().Be("some/directory");
        options.GeneratedImageDirectory.Should().Be("another/directory");
        options.ResetEnginesWithoutState.Should().BeFalse();
        options.StopOnLastUserDisconnect.Should().BeFalse();
        options.Database.ConnectionString.Should().Be("IAmAConnectionString");
    }

    [Test]
    public void TestDccExSerialOptions()
    {
        ConfigurationBuilder builder = new();
        builder.AddJsonFile("./Config/testConfig.json");
        var config = builder.Build();

        var options = config.GetRequiredSection("CommandStation").Get<DccExSerialOptions>();

        options.BaudRate.Should().Be(115200);
        options.SerialPort.Should().Be("/dev/ttyACM0");
    }
}
