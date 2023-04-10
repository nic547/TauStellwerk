﻿// <copyright file="EngineServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Server;
using TauStellwerk.Server.CommandStations;
using TauStellwerk.Server.Data.Model;
using TauStellwerk.Server.Services;

namespace TauStellwerk.Test.Services.EngineControlService;

public class EngineServiceTests
{
    private readonly Engine _engine = new()
    {
        Id = 1,
        Functions = new List<DccFunction>
        {
            new(0, "Light", 0),
        },
    };

    [Test]
    public async Task CanAcquireEngineTest()
    {
        var (service, session) = PrepareEngineService();

        var result = await service.AcquireEngine(session, _engine);

        Assert.That(result.IsSuccess);
    }

    [Test]
    public async Task CannotAcquireEngineTest()
    {
        var mock = GetAlwaysTrueMock();
        mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(Result.Fail("Fail"));
        var (service, session) = PrepareEngineService(mock);

        var result = await service.AcquireEngine(session, _engine);

        result.Should().BeFailure(string.Empty);
    }

    [Test]
    public async Task CannotAcquireEngineTwice()
    {
        var (service, session) = PrepareEngineService();

        var firstResult = await service.AcquireEngine(session, _engine);
        var secondResult = await service.AcquireEngine(session, _engine);

        firstResult.Should().BeSuccess();
        secondResult.Should().BeFailure();
    }

    [Test]
    public async Task CanReleaseAcquiredEngine()
    {
        var (service, session) = PrepareEngineService();

        var acquireResult = await service.AcquireEngine(session, _engine);
        var releaseResult = await service.ReleaseEngine(session, _engine.Id);

        acquireResult.Should().BeSuccess();
        releaseResult.Should().BeSuccess();
    }

    [Test]
    public async Task CannotReleaseEngineTwice()
    {
        var (service, session) = PrepareEngineService();

        _ = await service.AcquireEngine(session, _engine);
        var firstRelease = await service.ReleaseEngine(session, _engine.Id);
        var secondRelease = await service.ReleaseEngine(session, _engine.Id);

        firstRelease.Should().BeSuccess();
        secondRelease.Should().BeFailure();
    }

    [Test]
    public async Task DifferentSessionCannotRelease()
    {
        var (service, session) = PrepareEngineService();

        var session2 = new Session("differentConnection", "TestUser");

        var acquireResult = await service.AcquireEngine(session, _engine);
        var releaseResult = await service.ReleaseEngine(session2, _engine.Id);

        acquireResult.Should().BeSuccess();
        releaseResult.Should().BeFailure();
    }

    [Test]
    public async Task CanAcquireReleasedEngine()
    {
        var (service, session) = PrepareEngineService();

        var acquireResult = await service.AcquireEngine(session, _engine);
        var releaseResult = await service.ReleaseEngine(session, _engine.Id);
        var reacquireResult = await service.AcquireEngine(session, _engine);

        acquireResult.Should().BeSuccess();
        releaseResult.Should().BeSuccess();
        reacquireResult.Should().BeSuccess();
    }

    [Test]
    public async Task CannotSetSpeedIfNotAcquired()
    {
        var (service, session) = PrepareEngineService();

        var speedResult = await service.SetEngineSpeed(session, 1, 100, null);

        speedResult.Should().BeFailure();
    }

    [Test]
    public async Task CanSetSpeedIfAcquired()
    {
        var (service, session) = PrepareEngineService();

        _ = await service.AcquireEngine(session, _engine);
        var speedResult = await service.SetEngineSpeed(session, 1, 100, null);

        speedResult.Should().BeSuccess();
    }

    [Test]
    public async Task CannotSetSpeedWithInvalidSession()
    {
        var (service, session) = PrepareEngineService();
        var session2 = new Session("connnnectioniiiiidd", "TestUser");

        await service.AcquireEngine(session, _engine);
        var speedResult = await service.SetEngineSpeed(session2, 1, 100, null);

        speedResult.Should().BeFailure();
    }

    [Test]
    public async Task CannotSetFunctionIfNotAcquired()
    {
        var (service, session) = PrepareEngineService();

        var functionResult = await service.SetEngineFunction(session, 1, 2, State.Off);

        functionResult.Should().BeFailure();
    }

    [Test]
    public async Task CanSetFunctionIfAcquired()
    {
        var (service, session) = PrepareEngineService();

        await service.AcquireEngine(session, _engine);
        var functionResult = await service.SetEngineFunction(session, 1, 0, State.On);

        functionResult.Should().BeSuccess();
    }

    [Test]
    public async Task CannotSetFunctionWithInvalidSession()
    {
        var (service, session) = PrepareEngineService();
        var session2 = new Session("almostanId", "TestUser");

        await service.AcquireEngine(session, _engine);
        var functionResult = await service.SetEngineFunction(session2, 1, 10, State.On);

        functionResult.Should().BeFailure();
    }

    private static (Server.Services.EngineControlService EngineService, Session Session) PrepareEngineService(Mock<CommandStationBase>? mock = null)
    {
        var logger = new Mock<ILogger<SessionService>>();

        var sessionService = new SessionService(logger.Object);
        var session = new Session("ConnectionId", "TestUser");
        sessionService.HandleConnected(session.ConnectionId, session.UserName);
        mock ??= GetAlwaysTrueMock();
        var loggerMock = new Mock<ILogger<Server.Services.EngineControlService>>();

        TauStellwerkOptions options = new()
        {
            ResetEnginesWithoutState = true,
        };

        Server.Services.EngineControlService engineControlControlService = new(mock.Object, sessionService, loggerMock.Object, Options.Create(options));
        return (engineControlControlService, session);
    }

    private static Mock<CommandStationBase> GetAlwaysTrueMock()
    {
        var mock = new Mock<CommandStationBase>();
        mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(Result.Ok());
        mock.Setup(e => e.TryReleaseEngine(It.IsAny<Engine>(), It.IsAny<EngineState>()).Result).Returns(true);

        return mock;
    }
}