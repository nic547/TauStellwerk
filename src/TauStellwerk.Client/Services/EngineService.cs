// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
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

    public async Task<IReadOnlyList<EngineDto>> GetEngines(int page = 0, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true, bool showHidden = false)
    {
        var connection = await _service.GetHubConnection();

        return await connection.InvokeAsync<IReadOnlyList<EngineDto>>("GetEngines", page, sorting, sortDescending, showHidden);
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

    public async Task SetEStop(int id)
    {
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineEStop", id);
    }

    public async Task SetFunction(int id, byte function, State state)
    {
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineFunction", id, function, state);
    }

    public async Task AddOrUpdateEngine(EngineFull engine)
    {
        var engineDto = engine.ToDto();
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("AddOrUpdateEngine", engineDto);
    }

    private async Task SendSpeed((int Id, int Speed, Direction Direction) arg)
    {
        var (id, speed, direction) = arg;
        var connection = await _service.GetHubConnection();
        await connection.SendAsync("SetEngineSpeed", id, speed, direction);
    }
}