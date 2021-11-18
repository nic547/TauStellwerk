// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TauStellwerk.Base;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Client.Services;

public class EngineService
{
    private const double TimeoutMilliseconds = 100;

    private readonly IConnectionService _service;
    private readonly Dictionary<int, CoalescingLimiter<(int, int, bool)>> _activeEngines = new();

    public EngineService(IConnectionService httpClientService)
    {
        _service = httpClientService;
    }

    public async Task<IReadOnlyList<EngineDto>> GetEngines(int page = 0, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true, bool showHidden = false)
    {
        // var client = await _service.GetHttpClient();
        // var response = await client.GetAsync($"/engine/list?page={page}&sortBy={sorting}&sortDescending={sortDescending}&showHiddenEngines={showHidden}");
        // var responseString = await response.Content.ReadAsStringAsync();
        // var engines = JsonSerializer.Deserialize(responseString, TauJsonContext.Default.EngineDtoArray) ?? Array.Empty<EngineDto>();

        return Array.Empty<EngineDto>();
    }

    public async Task<EngineFull?> AcquireEngine(int id)
    {
        // var client = await _service.GetHttpClient();
        // var engineTask = client.GetAsync($"/engine/{id}");
        // var acquireResult = await client.PostAsync($"/engine/{id}/acquire", new StringContent(string.Empty));

        // if (acquireResult.StatusCode == HttpStatusCode.OK)
        // {
        //     var response = await engineTask;
        //     var json = await response.Content.ReadAsStringAsync();
        //     _activeEngines.Add(id, new CoalescingLimiter<(int, int, bool)>(SendSpeed, TimeoutMilliseconds));
        //     var dto = JsonSerializer.Deserialize(json, TauJsonContext.Default.EngineFullDto);
        //     return EngineFull.Create(dto);
        // }

        return null;
    }

    public async Task ReleaseEngine(int id)
    {
        _activeEngines.Remove(id);
        var client = await _service.GetHubConnection();
        //await client.PostAsync($"/engine/{id}/release", new StringContent(string.Empty));
    }

    public async Task SetSpeed(int id, int speed, bool forward)
    {
        _ = _activeEngines.TryGetValue(id, out var limiter);
        if (limiter == null)
        {
            throw new InvalidOperationException();
        }

        await limiter.Execute((id, speed, forward));
    }

    public async Task SetEStop(int id)
    {
        var client = await _service.GetHubConnection();
        // await client.PostAsync($"/engine/{id}/estop", new StringContent(string.Empty));
    }

    public async Task SetFunction(int id, byte function, bool on)
    {
        var client = await _service.GetHubConnection();
        var path = $"/engine/{id}/function/{function}/{(on ? "on" : "off")}";

        // await client.PostAsync(path, new StringContent(string.Empty));
    }

    public async Task AddOrUpdateEngine(EngineFull engine)
    {
        var client = await _service.GetHubConnection();
        var engineDto = engine.ToDto();
        // await client.PostAsync("/engine", new StringContent(JsonSerializer.Serialize(engineDto, TauJsonContext.Default.EngineFullDto), Encoding.UTF8, "text/json"));
    }

    private async Task SendSpeed((int Id, int Speed, bool Forward) arg)
    {
        var (id, speed, forward) = arg;
        var client = await _service.GetHubConnection();
        var path = $"/engine/{id}/speed/{speed}?forward={forward}";
        // await client.PostAsync(path, new StringContent(string.Empty));
    }
}