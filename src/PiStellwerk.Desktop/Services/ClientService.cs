// <copyright file="ClientService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Splat;

namespace PiStellwerk.Desktop.Services
{
    public class ClientService
    {
        private readonly SettingsService _settingsService;

        private readonly Timer _sessionTimer;
        private string _sessionId = string.Empty;

        public ClientService(SettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<SettingsService>();

            _sessionTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _sessionTimer.Elapsed += KeepSessionAlive;
            _sessionTimer.AutoReset = true;
        }

        public async Task<HttpClient> GetHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            };

            var baseAddress = new Uri((await _settingsService.GetSettings()).ServerAddress);

            var client = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
            };

            var sessionId = await GetSessionId(client);

            client.DefaultRequestHeaders.TryAddWithoutValidation("session-id", sessionId);

            return client;
        }

        private async Task<string> GetSessionId(HttpClient client)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                var username = (await _settingsService.GetSettings()).Username;
                var response = await client.PostAsync("/session", new StringContent($"\"{username}\"", Encoding.UTF8, "text/json"));
                _sessionId = await response.Content.ReadAsStringAsync();

                _sessionTimer.Enabled = true;
            }

            return _sessionId;
        }

        private async void KeepSessionAlive(object source, ElapsedEventArgs e)
        {
            var client = await GetHttpClient();
            _ = await client.PutAsync("/session", new StringContent(string.Empty));
        }
    }
}
