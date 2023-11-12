// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base;
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

    public event EventHandler<EngineFull>? EngineChanged;

    public async Task<IReadOnlyList<EngineOverview>> GetEngines(string searchTerm = "", int page = 0, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true, bool showHidden = false)
    {
        var connection = await _service.TryGetHubConnection();
        if (connection is null)
        {
            return new List<EngineOverview>();
        }

        var engines = await connection.InvokeAsync<List<EngineOverviewDto>>("GetEngines", searchTerm, page, sorting, sortDescending, showHidden);
        return engines.Select(e => new EngineOverview(e)).ToList();
    }

    public async Task<EngineFull?> AcquireEngine(int id)
    {
        var connection = await _service.TryGetHubConnection();
        if (connection == null)
        {
            return null;
        }

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
        var connection = await _service.TryGetHubConnection();
        if (connection != null)
        {
            await connection.SendAsync("ReleaseEngine", id);
        }
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
        var connection = await _service.TryGetHubConnection();
        if (connection != null)
        {
            await connection.SendAsync("SetEngineEStop", id);
        }
    }

    public async Task SetEStop(EngineFull activeEngine)
    {
        activeEngine.Throttle = 0;
        await SetEStop(activeEngine.Id);
    }

    public async Task SetFunction(int id, byte function, State state)
    {
        var connection = await _service.TryGetHubConnection();
        if (connection != null)
        {
            await connection.SendAsync("SetEngineFunction", id, function, state);
        }
    }

    public async Task ToggleFunction(EngineFull engine, Function function)
    {
        if (function.Duration > 0 && function.State == State.On)
        {
            // Momentary functions don't need to be "turned off", that's handled by the server.
            return;
        }

        var connection = await _service.TryGetHubConnection();

        if (connection is null)
        {
            return;
        }

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

    public async Task<EngineFull> AddOrUpdateEngine(EngineFull engine)
    {
        var engineDto = engine.ToDto();
        var connection = await _service.TryGetHubConnection() ?? throw new InvalidOperationException();

        var updateResult = await connection.InvokeAsync<ResultDto<EngineFullDto>>("AddOrUpdateEngine", engineDto);
        if (!updateResult.Success)
        {
            throw new InvalidOperationException(updateResult.Error);
        }

        var updatedEngine = new EngineFull(updateResult.Value);
        EngineChanged?.Invoke(this, updatedEngine);
        return updatedEngine;
    }

    public async Task UpdateEngineImage(EngineFull engine, MemoryStream stream, string filename)
    {
        HttpClient client = await _service.TryGetHttpClient() ?? throw new InvalidOperationException();

        ByteArrayContent arrayContent = new(stream.ToArray());
        MultipartFormDataContent content = new()
        {
            { arrayContent, "image", filename },
        };

        await client.PostAsync($"/upload/{engine.Id}", content);
    }

    public async Task<ResultDto> TryDeleteEngine(EngineFull engine)
    {
        if (engine.Id == 0)
        {
            return new ResultDto(false, "Engine does not exist yet");
        }

        var connection = await _service.TryGetHubConnection();
        if (connection is null)
        {
            return new ResultDto(false, "Failed to establish connection.");
        }

        _activeEngines.Remove(engine.Id);
        return await connection.InvokeAsync<ResultDto>("DeleteEngine", engine.Id);
    }

    private async Task SendSpeed((int Id, int Speed, Direction Direction) arg)
    {
        var (id, speed, direction) = arg;
        var connection = await _service.TryGetHubConnection();
        if (connection is null)
        {
            return;
        }

        await connection.SendAsync("SetEngineSpeed", id, speed, direction);
    }
}