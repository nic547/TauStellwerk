// <copyright file="StatusIntegrationTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests;

public class StatusIntegrationTests : IntegrationTestBase
{
    [Test]
    [Category("long-running")]
    public async Task StatusChangeEventsAreEmitted()
    {
        var service1 = CreateStatusService();
        var service2 = CreateStatusService();

        SystemStatus? lastStatusNotification = null;
        service1.StatusChanged += (_, status) => { lastStatusNotification = status; };

        await service2.SetStatus(SystemStatus(State.On, "Roobärt"));

        // Wait for SignalR to actually send the event to the other connections.
        await Task.Delay(100);

        lastStatusNotification!.State.Should().Be(State.On);
        lastStatusNotification.LastActionUsername.Should().Be("Roobärt");
        service1.LastKnownStatus!.State.Should().Be(State.On);
        service2.LastKnownStatus!.State.Should().Be(State.On);

        await service2.SetStatus(SystemStatus(State.Off, "You"));

        // Wait for SignalR to actually send the event to the other connections.
        await Task.Delay(100);

        lastStatusNotification!.State.Should().Be(State.Off);
        lastStatusNotification.LastActionUsername.Should().Be("You");
        service1.LastKnownStatus!.State.Should().Be(State.Off);
        service2.LastKnownStatus!.State.Should().Be(State.Off);
    }

    private static SystemStatus SystemStatus(State state, string username)
    {
        return new SystemStatus()
        {
            State = state,
            LastActionUsername = username,
        };
    }

    private StatusService CreateStatusService()
    {
        return new StatusService(CreateConnectionService());
    }
}