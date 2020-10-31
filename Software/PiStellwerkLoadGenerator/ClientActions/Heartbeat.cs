// <copyright file="Heartbeat.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PiStellwerk.Data;

namespace PiStellwerkLoadGenerator.ClientActions
{
    /// <summary>
    /// The regular Heartbeat sent by a client to ensure a working connection to the server.
    /// </summary>
    public class Heartbeat : IClientAction
    {
        private Random _random;

        private HttpClient _client;
        private Options _options;
        private User _user;

        /// <inheritdoc/>
        public int Interval => _random.Next(1900, 2100);

        /// <inheritdoc />
        public void Initialize(HttpClient client, Options options, Random random)
        {
            _client = client;
            _options = options;
            _random = random;

            _user = new User()
            {
                Name = _random.Next(1, 999999999).ToString(),
                UserAgent = "PiStellwerk ",
            };
        }

        /// <inheritdoc/>
        public async Task<int> PerformRequest()
        {
            var startTime = DateTime.Now;
            var content = new StringContent(JsonSerializer.Serialize(_user), Encoding.UTF8, "application/json");
            _ = await _client.PutAsync(_options.Uri + "status", content);
            return (int)Math.Round((DateTime.Now - startTime).TotalMilliseconds);
        }
    }
}
