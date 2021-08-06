// <copyright file="HttpClientService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

namespace TauStellwerk.Client.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly ISettingsService _settingsService;

        private readonly Timer _sessionTimer;

        private string _sessionId = string.Empty;

        private HttpClient? _httpClient;

        public HttpClientService(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            _sessionTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _sessionTimer.Elapsed += KeepSessionAlive;
            _sessionTimer.AutoReset = true;
        }

        public async Task<HttpClient> GetHttpClient()
        {
            if (_httpClient != null)
            {
                return _httpClient;
            }

            var handler = new HttpClientHandler();

            try
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            }
            catch (NotSupportedException)
            {
                // Not all platforms support the certificate handling, for example Blazor WebAssembly doesn't.
                // Luckily the invalid certificates are handled by the browser in that case.
            }

            var baseAddress = new Uri((await _settingsService.GetSettings()).ServerAddress);

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
            };

            var sessionId = await GetSessionId(_httpClient);

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("session-id", sessionId);

            return _httpClient;
        }

        private async Task<string> GetSessionId(HttpClient client)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                var username = (await _settingsService.GetSettings()).Username;
                var response = await client.PostAsync(
                    "/session",
                    JsonContent.Create(username));
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
