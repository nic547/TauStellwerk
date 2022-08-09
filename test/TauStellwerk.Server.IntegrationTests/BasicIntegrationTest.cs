// <copyright file="BasicIntegrationTest.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests;

public class BasicIntegrationTest
{
    private WebApplicationFactory<Startup> _factory = null!; // Created in setup

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Startup>();
    }

    [Test]
    [Category("long-running")]
    public async Task CanCreateAndLoadEngines()
    {
        var engineService = new EngineService(CreateConnectionService());

        var enginesToInsert = Tools.CreateTestDb.EngineDtoGenerator.GetEngineFullDtos(100);
        foreach (var engine in enginesToInsert)
        {
            await engineService.AddOrUpdateEngine(new EngineFull(engine));
        }

        List<EngineOverview> engines = new();

        foreach (var i in Enumerable.Range(0, 6))
        {
            engines.AddRange(await engineService.GetEngines(i, SortEnginesBy.Name, true));
        }

        engines.Should().HaveCount(100);
    }

    [Test]
    [Category("long-running")]
    public async Task CanCreateAndControlEngine()
    {
        var engineService = new EngineService(CreateConnectionService());

        var engineToInsert = Tools.CreateTestDb.EngineDtoGenerator.GetEngineDto();
        var engine = await engineService.AddOrUpdateEngine(new EngineFull(engineToInsert));

        await engineService.AcquireEngine(engine.Id);
        await engineService.SetSpeed(engine.Id, 20, Direction.Forwards);
        await engineService.SetEStop(engine.Id);
        await engineService.SetFunction(engine.Id, 1, State.On);
        await engineService.ReleaseEngine(engine.Id);

        Assert.Pass();
    }

    private IConnectionService CreateConnectionService()
    {
        var settingService = new Mock<ISettingsService>(MockBehavior.Strict);

        settingService.Setup(s => s.GetSettings().Result)
            .Returns(() => new ImmutableSettings("TEST", _factory.Server.BaseAddress.ToString(), string.Empty));

        var hubConnection = new HubConnectionBuilder().WithUrl(
            _factory.Server.BaseAddress + "hub",
            options => options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler()).Build();

        return new ConnectionService(settingService.Object, hubConnection);
    }
}
