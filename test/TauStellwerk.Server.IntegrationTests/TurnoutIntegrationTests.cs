// <copyright file="TurnoutIntegrationTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests;

public class TurnoutIntegrationTests : IntegrationTestBase
{
    [Test]
    [Category("long-running")]
    public async Task CanCreateAndToggleTurnout()
    {
        var turnoutService = new TurnoutService(CreateConnectionService());

        var turnoutToInsert = new Turnout()
        {
            Address = 230,
            Name = "Test Turnout",
            Kind = TurnoutKind.SlimLeftTurnout,
        };

        await turnoutService.AddOrUpdate(turnoutToInsert);

        var turnoutList = await turnoutService.GetList();
        var turnout = turnoutList[0];

        await turnoutService.ToggleState(turnout);
        await turnoutService.ToggleState(turnout);
        await turnoutService.ToggleState(turnout);

        turnout.State.Should().Be(State.On);
        turnout.Address.Should().Be(230);
        turnout.Name.Should().Be("Test Turnout");
        turnout.Kind.Should().Be(TurnoutKind.SlimLeftTurnout);
    }
}