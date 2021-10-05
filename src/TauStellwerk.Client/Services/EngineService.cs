// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Client.Services
{
    public class EngineService
    {
        private const double TimeoutMiliseconds = 100;

        private readonly IHttpClientService _service;
        private readonly Dictionary<int, CoalescingLimiter<(int, int, bool)>> _activeEngines = new();
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public EngineService(IHttpClientService httpClientService)
        {
            _service = httpClientService;
        }

        public async Task<IReadOnlyList<EngineDto>> GetEngines(int page = 0, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true)
        {
            var client = await _service.GetHttpClient();
            var response = await client.GetAsync($"/engine/list?page={page}&sortBy={sorting}&sortDescending={sortDescending}");
            var responseString = await response.Content.ReadAsStringAsync();
            var engines = JsonSerializer.Deserialize<EngineDto[]>(responseString, _serializerOptions) ?? System.Array.Empty<EngineDto>();

            return engines;
        }

        public async Task<EngineFull?> AcquireEngine(int id)
        {
            var client = await _service.GetHttpClient();
            var engineTask = client.GetAsync($"/engine/{id}");
            var acquireResult = await client.PostAsync($"/engine/{id}/acquire", new StringContent(string.Empty));

            if (acquireResult.StatusCode == HttpStatusCode.OK)
            {
                var response = await engineTask;
                var json = await response.Content.ReadAsStringAsync();
                _activeEngines.Add(id, new CoalescingLimiter<(int, int, bool)>(SendSpeed, TimeoutMiliseconds));
                var dto = JsonSerializer.Deserialize<EngineFullDto>(json, _serializerOptions);
                return EngineFull.Create(dto);
            }

            return null;
        }

        public async Task ReleaseEngine(int id)
        {
            _activeEngines.Remove(id);
            var client = await _service.GetHttpClient();
            await client.PostAsync($"/engine/{id}/release", new StringContent(string.Empty));
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
            var client = await _service.GetHttpClient();
            await client.PostAsync($"/engine/{id}/estop", new StringContent(string.Empty));
        }

        public async Task SetFunction(int id, byte function, bool on)
        {
            var client = await _service.GetHttpClient();
            var path = $"/engine/{id}/function/{function}/{(on ? "on" : "off")}";

            await client.PostAsync(path, new StringContent(string.Empty));
        }

        public async Task AddOrUpdateEngine(EngineFull engine)
        {
            var client = await _service.GetHttpClient();
            var engineDto = engine.ToDto();
            await client.PostAsync("/engine", JsonContent.Create(engineDto, typeof(EngineFullDto), null, _serializerOptions));
        }

        private async Task SendSpeed((int Id, int Speed, bool Forward) arg)
        {
            var (id, speed, forward) = arg;
            var client = await _service.GetHttpClient();
            var path = $"/engine/{id}/speed/{speed}?forward={forward}";
            await client.PostAsync(path, new StringContent(string.Empty));
        }
    }
}
