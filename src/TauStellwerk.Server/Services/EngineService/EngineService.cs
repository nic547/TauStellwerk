// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.Server.CommandStations;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.Services.EngineService;

public class EngineService : IEngineService
{
    private const int CheckMomentaryFunctionsEveryMilliseconds = 100;

    private readonly CommandStationBase _commandStation;
    private readonly ILogger _logger;
    private readonly TauStellwerkOptions _options;

    private readonly EngineManager _manager;
    private readonly MomentaryFunctionHandler _momentaryFunctionHandler;

    public EngineService(CommandStationBase commandStation, SessionService sessionService, ILogger<EngineService> logger, IOptions<TauStellwerkOptions> options)
    {
        _commandStation = commandStation;
        _logger = logger;
        _manager = new EngineManager(logger);
        _momentaryFunctionHandler = new MomentaryFunctionHandler(commandStation, CheckMomentaryFunctionsEveryMilliseconds);
        _options = options.Value;

        sessionService.SessionTimeout += _manager.HandleSessionTimeout;
    }

    public async Task<Result<EngineFullDto>> AcquireEngine(Session session, Engine engine)
    {
        var engineManagerResult = _manager.AddActiveEngine(engine, session);
        if (engineManagerResult.IsFailed)
        {
            return engineManagerResult.ToResult();
        }

        var systemResult = await _commandStation.TryAcquireEngine(engine);

        if (systemResult == false)
        {
            _logger.LogWarning("{session} tried acquiring {engine}, but the command system returned false", session, engine);
            _manager.RemoveActiveEngine(engine.Id, session);
            return Result.Fail("Engine already in use");
        }

        _logger.LogInformation("{session} acquired {engine}", session, engine);

        if (_options.ResetEnginesWithoutState && engineManagerResult.Value.IsNew)
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

        var systemReleaseSuccess = await _commandStation.TryReleaseEngine(activeEngineResult.Value.Engine, activeEngineResult.Value.State);
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
        await _commandStation.HandleEngineSpeed(activeEngine.Engine, (short)speed, priorDirection, activeEngine.State.Direction);
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

        await _commandStation.HandleEngineEStop(activeEngine.Engine, activeEngine.State.Direction);
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

        await _commandStation.HandleEngineFunction(activeEngine.Engine, (byte)functionNumber, state);

        var currentFunction = activeEngine.Engine.Functions.Single(f => f.Number == functionNumber);

        if (currentFunction.Duration > 0 && state == State.On)
        {
            _momentaryFunctionHandler.Add(activeEngine.Engine, currentFunction);
        }
        else
        {
            activeEngine.State.FunctionStates[functionNumber] = state;
        }

        return Result.Ok();
    }

    public Result IsEngineAcquiredBySession(Session session, int engineId)
    {
        return _manager.GetActiveEngine(engineId, session).ToResult();
    }

    private async Task ResetEngine(Engine engine)
    {
        await _commandStation.HandleEngineSpeed(engine, 0, Direction.Backwards, Direction.Forwards);
        foreach (var function in engine.Functions)
        {
            await _commandStation.HandleEngineFunction(engine, function.Number, State.Off);
        }
    }
}