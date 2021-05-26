// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PiStellwerk.Data;
using Splat;

namespace PiStellwerk.Desktop.Services
{
    public class StatusService
    {
        private ClientService _clientService;

        public StatusService(ClientService? clientService = null)
        {
            _clientService = clientService ?? Locator.Current.GetService<ClientService>();

            Task.Run(async () => { await TrackStatus(); });
        }

        public delegate void StatusChangeHandler(Status status);

        public event StatusChangeHandler? StatusChanged;

        public Status? LastKnownStatus { get; private set; }

        private CancellationToken CancellationToken { get; } = CancellationToken.None;

        public async Task SetStatus(Status status)
        {
            var client = await _clientService.GetHttpClient();
            var json = JsonSerializer.Serialize(status);
            StatusChanged?.Invoke(status);
            LastKnownStatus = status;
            _ = await client.PostAsync("/status", new StringContent(json, Encoding.UTF8, "text/json"));
        }

        private async Task TrackStatus()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                var status = await GetStatus(LastKnownStatus?.IsRunning);
                if (status != null)
                {
                    StatusChanged?.Invoke(status);
                    LastKnownStatus = status;
                }
            }
        }

        private async Task<Status?> GetStatus(bool? lastState)
        {
            var client = await _clientService.GetHttpClient();
            var uri = lastState == null ? "/status" : $"/status?lastKnownStatus={lastState}";
            var response = await client.GetAsync(uri, CancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Status>(responseContent, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
    }
}