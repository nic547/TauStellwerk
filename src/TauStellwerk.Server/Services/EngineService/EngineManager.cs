using System.Collections.Concurrent;
using FluentResults;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Services.EngineService;

public class EngineManager
{
    private readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

    private readonly ConcurrentDictionary<int, EngineState> _inactiveEngineStates = new();

    private readonly ILogger<EngineService> _logger;

    public EngineManager(ILogger<EngineService> logger)
    {
        _logger = logger;
    }

    public Result<EngineFullDto> AddActiveEngine(Engine engine, Session session)
    {
        _inactiveEngineStates.TryGetValue(engine.Id, out var state);

        if (state == null || engine.Functions.Count != state.FunctionStates.Count)
        {
            state = new EngineState(engine.Functions.Count);
        }

        var success = _activeEngines.TryAdd(engine.Id, new ActiveEngine(session, engine, state));

        if (!success)
        {
            return Result.Fail("Engine already in use.");
        }

        var dto = engine.ToEngineFullDto();
        state.UpdateEngineFullDto(ref dto);

        _inactiveEngineStates.TryRemove(engine.Id, out _);

        return Result.Ok(dto);
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
            _logger.LogError($"{session} tried accessing {activeEngine.Engine}");
            return Result.Fail("Wrong sessios for engine given.");
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
        _logger.LogInformation($"{session} released {activeEngine.Engine}");

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
                    _logger.LogWarning($"Released {active.Engine} because {session.UserName} disconnected!");
                }
            }
        }
    }
}