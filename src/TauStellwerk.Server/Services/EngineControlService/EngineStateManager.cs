// <copyright file="EngineStateManager.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services;

public class EngineStateManager
{
    private readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

    private readonly ConcurrentDictionary<int, EngineState> _inactiveEngineStates = new();

    private readonly ILogger<EngineControlService> _logger;

    public EngineStateManager(ILogger<EngineControlService> logger)
    {
        _logger = logger;
    }

    public Result<(EngineFullDto Engine, bool IsNew)> AddActiveEngine(Engine engine, Session session)
    {
        _inactiveEngineStates.TryGetValue(engine.Id, out var state);
        var isStateNew = false;

        if (state == null || engine.Functions.Count != state.FunctionStates.Count)
        {
            state = new EngineState(engine.Functions.Count);
            isStateNew = true;
        }

        var success = _activeEngines.TryAdd(engine.Id, new ActiveEngine(session, engine, state));

        if (!success)
        {
            return Result.Fail("Engine already in use.");
        }

        var dto = engine.ToEngineFullDto();
        state.UpdateEngineFullDto(ref dto);

        _inactiveEngineStates.TryRemove(engine.Id, out _);

        return Result.Ok((dto, isStateNew));
    }

    public Result<ActiveEngine> GetActiveEngine(int id, Session session)
    {
        _activeEngines.TryGetValue(id, out var activeEngine);

        if (activeEngine == null)
        {
            return Result.Fail("Engine is not active");
        }

        if (activeEngine.Session != session)
        {
            _logger.LogError("{session} tried accessing {engine}", session, activeEngine.Engine);
            return Result.Fail("Wrong session for engine given.");
        }

        return Result.Ok(activeEngine);
    }

    public Result<ActiveEngine> RemoveActiveEngine(int id, Session session)
    {
        var activeEngineResult = GetActiveEngine(id, session);
        if (activeEngineResult.IsFailed)
        {
            return activeEngineResult.ToResult();
        }

        var activeEngine = activeEngineResult.Value;

        _activeEngines.TryRemove(id, out _);

        _inactiveEngineStates.TryAdd(id, activeEngine.State);
        _logger.LogInformation("{session} released {engine}", session, activeEngine.Engine);

        return Result.Ok(activeEngine);
    }

    public void HandleSessionTimeout(Session session)
    {
        foreach (var active in _activeEngines.Values)
        {
            if (active.Session == session)
            {
                _activeEngines.TryRemove(active.Engine.Id, out var engine);
                if (engine != null)
                {
                    _inactiveEngineStates.TryAdd(engine.Engine.Id, engine.State);
                    _logger.LogWarning("Released {engine} because {userName} disconnected!", active.Engine, session.UserName);
                }
            }
        }
    }
}