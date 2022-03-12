// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Services.EngineService;

public class EngineService : IEngineService
{
    private readonly CommandSystemBase _commandSystem;
    private readonly ILogger _logger;

    private readonly EngineManager _manager;

    private readonly bool _resetEnginesWithoutState;

    public EngineService(CommandSystemBase commandSystem, SessionService sessionService, ILogger<EngineService> logger, IConfiguration config)
    {
        _commandSystem = commandSystem;
        _logger = logger;
        _manager = new EngineManager(logger);

        _resetEnginesWithoutState = config.GetValue("ResetEnginesWithoutState", true);

        sessionService.SessionTimeout += _manager.HandleSessionTimeout;
    }

    public async Task<Result<EngineFullDto>> AcquireEngine(Session session, Engine engine)
    {
        var engineManagerResult = _manager.AddActiveEngine(engine, session);
        if (engineManagerResult.IsFailed)
        {
            return engineManagerResult.ToResult();
        }

        var systemResult = await _commandSystem.TryAcquireEngine(engine);

        if (systemResult == false)
        {
            _logger.LogWarning($"{session} tried acquiring {engine}, but the command system returned false");
            _manager.RemoveActiveEngine(engine.Id, session);
            return Result.Fail("Engine already in use");
        }

        _logger.LogInformation($"{session} acquired {engine}");

        if (_resetEnginesWithoutState && engineManagerResult.Value.IsNew)
        {
            await ResetEngine(engine);
        }

        return Result.Ok(engineManagerResult.Value.Engine);
    }

    public async Task<Result> ReleaseEngine(Session session, int engineId)
    {
        var activeEngineResult = _manager.RemoveActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var systemReleaseSuccess = await _commandSystem.TryReleaseEngine(activeEngineResult.Value.Engine);
        return systemReleaseSuccess ? Result.Ok() : Result.Fail("CommandSystem could not release engine");
    }

    public async Task<Result> SetEngineSpeed(Session session, int engineId, int speed, Direction? newDirection)
    {
        var activeEngineResult = _manager.GetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        var priorDirection = activeEngine.State.Direction;
        activeEngine.State.Direction = newDirection ?? priorDirection;
        activeEngine.State.Throttle = speed;
        await _commandSystem.HandleEngineSpeed(activeEngine.Engine, (short)speed, priorDirection, activeEngine.State.Direction);
        return Result.Ok();
    }

    public async Task<Result> SetEngineEStop(Session session, int engineId)
    {
        var activeEngineResult = _manager.GetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;
        activeEngine.State.Throttle = 0;

        await _commandSystem.HandleEngineEStop(activeEngine.Engine, activeEngine.State.Direction);
        return Result.Ok();
    }

    public async Task<Result> SetEngineFunction(Session session, int engineId, int functionNumber, State state)
    {
        var activeEngineResult = _manager.GetActiveEngine(engineId, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        await _commandSystem.HandleEngineFunction(activeEngine.Engine, (byte)functionNumber, state);
        activeEngine.State.FunctionStates[functionNumber] = state;
        return Result.Ok();
    }

    public Result IsEngineAcquiredBySession(Session session, int engineId)
    {
        return _manager.GetActiveEngine(engineId, session).ToResult();
    }

    private async Task ResetEngine(Engine engine)
    {
        await _commandSystem.HandleEngineSpeed(engine, 0, Direction.Backwards, Direction.Forwards);
        foreach (var function in engine.Functions)
        {
            await _commandSystem.HandleEngineFunction(engine, function.Number, State.Off);
        }
    }
}