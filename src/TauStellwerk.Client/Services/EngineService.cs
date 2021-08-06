// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Services
{
    public class EngineService
    {
        private readonly IHttpClientService _service;

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

        public async Task<EngineFullDto?> AcquireEngine(int id)
        {
            var client = await _service.GetHttpClient();
            var engineTask = client.GetAsync($"/engine/{id}");
            var acquireResult = await client.PostAsync($"/engine/{id}/acquire", new StringContent(string.Empty));

            if (acquireResult.StatusCode == HttpStatusCode.OK)
            {
                var response = await engineTask;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EngineFullDto>(json, _serializerOptions);
            }

            return null;
        }

        public async Task ReleaseEngine(int id)
        {
            var client = await _service.GetHttpClient();
            await client.PostAsync($"/engine/{id}/release", new StringContent(string.Empty));
        }

        public async Task SetSpeed(int id, int speed, bool? forward)
        {
            var client = await _service.GetHttpClient();
            var path = $"/engine/{id}/speed/{speed}";
            if (forward != null)
            {
                path += $"?forward={forward}";
            }

            _ = await client.PostAsync(path, new StringContent(string.Empty));
        }

        public async Task SetFunction(int id, byte function, bool on)
        {
            var client = await _service.GetHttpClient();
            var path = $"/engine/{id}/function/{function}/{(on ? "on" : "off")}";

            _ = await client.PostAsync(path, new StringContent(string.Empty));
        }
    }
}
