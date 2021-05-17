// <copyright file="ClientService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PiStellwerk.Desktop.Services
{
    public class ClientService
    {
        // TODO: REMOVE FIXED SHIT
        private const string _username = "\"REPLACEME\"";
        private const string _target = "https://192.168.1.10";

        private string _sessionId = string.Empty;

        public async Task<HttpClient> GetHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_target),
            };

            var sessionId = await GetSessionId(client);
            client.DefaultRequestHeaders.TryAddWithoutValidation("session-id", sessionId);

            return client;
        }

        private async Task<string> GetSessionId(HttpClient client)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                var response = await client.PostAsync("/session", new StringContent(_username, Encoding.UTF8, "text/json"));
                _sessionId = await response.Content.ReadAsStringAsync();
            }

            return _sessionId;
        }
    }
}
