// <copyright file="IEngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentResults;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Database.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Services;

public interface IEngineService
{
    Task<Result> AcquireEngine(Session session, Engine engine);

    Task<Result> ReleaseEngine(Session session, int engineId);

    Task<Result> SetEngineSpeed(Session session, int engineId, int speed, bool? shouldDriveForward);

    Task<Result> SetEngineEStop(Session session, int engineId);

    Task<Result> SetEngineFunction(Session session, int engineId, int functionNumber, bool on);
}

public class EngineService : IEngineService
{
    private readonly CommandSystemBase _commandSystem;

    private readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

    public EngineService(CommandSystemBase commandSystem, SessionService sessionService)
    {
        _commandSystem = commandSystem;
        sessionService.SessionTimeout += HandleSessionTimeout;
    }

    public async Task<Result> AcquireEngine(Session session, Engine engine)
    {
        var serviceSuccess = _activeEngines.TryAdd(engine.Id, new ActiveEngine(session, engine));
        if (!serviceSuccess)
        {
            ConsoleService.PrintMessage($"{session} tried acquiring {engine}, but engine is already acquired.");
            return Result.Fail("Engine already in use");
        }

        var systemResult = await _commandSystem.TryAcquireEngine(engine);

        if (systemResult == false)
        {
            ConsoleService.PrintWarning($"{session} tried acquiring {engine}, but ICommandSystem returned false");
            _activeEngines.TryRemove(engine.Id, out _);
            return Result.Fail("Engine already in use");
        }

        ConsoleService.PrintMessage($"{session} acquired {engine}");

        return Result.Ok();
    }

    public async Task<Result> ReleaseEngine(Session session, int engineId)
    {
        var activeEngineResult = TryGetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        _activeEngines.TryRemove(engineId, out _);
        ConsoleService.PrintMessage($"{session} released {activeEngine.Engine}");

        var systemReleaseSuccess = await _commandSystem.TryReleaseEngine(activeEngine.Engine);
        return systemReleaseSuccess ? Result.Ok() : Result.Fail("CommandSystem could not release engine");
    }

    public async Task<Result> SetEngineSpeed(Session session, int engineId, int speed, bool? shouldBeDrivingForwards)
    {
        var activeEngineResult = TryGetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        var hasBeenDrivingForward = activeEngine.IsDrivingForward;
        shouldBeDrivingForwards ??= hasBeenDrivingForward;
        activeEngine.IsDrivingForward = (bool)shouldBeDrivingForwards;

        await _commandSystem.HandleEngineSpeed(activeEngine.Engine, (short)speed, hasBeenDrivingForward, (bool)shouldBeDrivingForwards);
        return Result.Ok();
    }

    public async Task<Result> SetEngineEStop(Session session, int engineId)
    {
        var activeEngineResult = TryGetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        var hasBeenDrivingForward = activeEngine.IsDrivingForward;
        await _commandSystem.HandleEngineEStop(activeEngine.Engine, hasBeenDrivingForward);
        return Result.Ok();
    }

    public async Task<Result> SetEngineFunction(Session session, int engineId, int functionNumber, bool on)
    {
        var activeEngineResult = TryGetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        await _commandSystem.HandleEngineFunction(activeEngine.Engine, (byte)functionNumber, on);
        return Result.Ok();
    }

    private Result<ActiveEngine> TryGetActiveEngine(int engineId, Session session)
    {
        _activeEngines.TryGetValue(engineId, out var activeEngine);
        if (activeEngine == null)
        {
            ConsoleService.PrintMessage($"{session} tried commanding engine with id {engineId}, but no such engine is active.");
            return Result.Fail($"No engine with id {engineId} was found");
        }

        if (activeEngine.Session != session)
        {
            ConsoleService.PrintWarning($"{session} tried something with {activeEngine.Engine}, but has wrong session");
            return Result.Fail("Wrong session");
        }

        return Result.Ok(activeEngine);
    }

    private void HandleSessionTimeout(Session session)
    {
        foreach (var active in _activeEngines.Values)
        {
            if (active.Session == session)
            {
                _activeEngines.TryRemove(active.Engine.Id, out var _);
                ConsoleService.PrintWarning($"Released {active.Engine} because {session.UserName} timed out!");
            }
        }
    }

    private class ActiveEngine
    {
        public ActiveEngine(Session session, Engine engine)
        {
            Session = session;
            Engine = engine;
        }

        public Session Session { get; }

        public Engine Engine { get; }

        public bool IsDrivingForward { get; set; } = true;
    }
}