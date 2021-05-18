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
using PiStellwerk.Data;
using Splat;

namespace PiStellwerk.Desktop.Services
{
    public class EngineService
    {
        private readonly ClientService _clientService;

        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public EngineService(ClientService? clientService = null)
        {
            _clientService = clientService ?? Locator.Current.GetService<ClientService>();
        }

        public async Task<IReadOnlyList<Engine>> GetEngines(int page = 0)
        {
            var client = await _clientService.GetHttpClient();
            var response = await client.GetAsync("/engine/list");
            var responseString = await response.Content.ReadAsStringAsync();
            var engines = JsonSerializer.Deserialize<Engine[]>(responseString, _serializerOptions) ?? System.Array.Empty<Engine>();

            return engines;
        }

        public async Task<Engine?> AcquireEngine(int id)
        {
            var client = await _clientService.GetHttpClient();
            var engineTask = client.GetAsync($"/engine/{id}");
            var acquireResult = await client.PostAsync($"/engine/{id}/acquire", new StringContent(string.Empty));

            if (acquireResult.StatusCode == HttpStatusCode.OK)
            {
                var response = await engineTask;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Engine>(json, _serializerOptions);
            }

            return null;
        }

        public async Task SetSpeed(int id, int speed, bool? forward)
        {
            var client = await _clientService.GetHttpClient();
            var path = $"/engine/{id}/speed/{speed}";
            if (forward != null)
            {
                path += $"?forward={forward}";
            }

            _ = await client.PostAsync(path, new StringContent(string.Empty));
        }

        public async Task SetFunction(int id, byte function, bool on)
        {
            var client = await _clientService.GetHttpClient();
            var path = $"/engine/{id}/function/{function}/{(on ? "on" : "off")}";

            _ = await client.PostAsync(path, new StringContent(string.Empty));
        }
    }
}
