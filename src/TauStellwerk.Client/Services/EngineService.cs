// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Client.Services;

public class EngineService
{
    private const double TimeoutMilliseconds = 100;

    private readonly IConnectionService _service;
    private readonly Dictionary<int, CoalescingLimiter<(int, int, Direction)>> _activeEngines = new();

    public EngineService(IConnectionService httpClientService)
    {
        _service = httpClientService;
    }

    public async Task<IReadOnlyList<EngineOverview>> GetEngines(int page = 0, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true, bool showHidden = false)
    {
        var connection = await _service.GetHubConnection();

        var engines = await connection.InvokeAsync<IReadOnlyList<EngineOverviewDto>>("GetEngines", page, sorting, sortDescending, showHidden);
        return engines.Select(e => new EngineOverview(e)).ToList();
    }

    public async Task<EngineFull?> AcquireEngine(int id)
    {
        var connection = await _service.GetHubConnection();
        var result = await connection.InvokeAsync<ResultDto<EngineFullDto>>("AcquireEngine", id);

        if (result.Success)
        {
            _activeEngines.Add(id, new CoalescingLimiter<(int, int, Direction)>(SendSpeed, TimeoutMilliseconds));
        }

        return EngineFull.Create(result.Value);
    }

    public async Task ReleaseEngine(int id)
    {
        _activeEngines.Remove(id);
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("ReleaseEngine", id);
    }

    public async Task SetSpeed(int id, int speed, Direction direction)
    {
        _ = _activeEngines.TryGetValue(id, out var limiter);
        if (limiter == null)
        {
            throw new InvalidOperationException();
        }

        await limiter.Execute((id, speed, direction));
    }

    public async Task SetSpeed(EngineFull activeEngine, int speed, Direction direction)
    {
        activeEngine.Direction = direction;
        activeEngine.Throttle = speed;
        await SetSpeed(activeEngine.Id, speed, direction);
    }

    public async Task SetEStop(int id)
    {
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineEStop", id);
    }

    public async Task SetEStop(EngineFull activeEngine)
    {
        activeEngine.Throttle = 0;
        await SetEStop(activeEngine.Id);
    }

    public async Task SetFunction(int id, byte function, State state)
    {
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineFunction", id, function, state);
    }

    public async Task ToggleFunction(EngineFull engine, Function function)
    {
        if (function.Duration > 0 && function.State == State.On)
        {
            // Momentary functions don't need to be "turned off", that's handled by the server.
            return;
        }

        var connection = await _service.GetHubConnection();
        if (function.State == State.Off)
        {
            await connection.SendAsync("SetEngineFunction", engine.Id, function.Number, State.On);
            function.State = State.On;
        }
        else
        {
            await connection.SendAsync("SetEngineFunction", engine.Id, function.Number, State.Off);
            function.State = State.Off;
        }
    }

    public async Task<EngineFullDto> AddOrUpdateEngine(EngineFull engine)
    {
        var engineDto = engine.ToDto();
        var connection = await _service.GetHubConnection();
        var updatedEngine = await connection.InvokeAsync<ResultDto<EngineFullDto>>("AddOrUpdateEngine", engineDto);

        // TODO: Handle Error instead of throwing.
        return updatedEngine.Value ?? throw new InvalidOperationException();
    }

    public async Task<ResultDto> TryDeleteEngine(EngineFull engine)
    {
        if (engine.Id == 0)
        {
            return new ResultDto(false, "Engine does not exist yet");
        }

        var connection = await _service.GetHubConnection();
        _activeEngines.Remove(engine.Id);
        return await connection.InvokeAsync<ResultDto>("DeleteEngine", engine.Id);
    }

    private async Task SendSpeed((int Id, int Speed, Direction Direction) arg)
    {
        var (id, speed, direction) = arg;
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineSpeed", id, speed, direction);
    }
}