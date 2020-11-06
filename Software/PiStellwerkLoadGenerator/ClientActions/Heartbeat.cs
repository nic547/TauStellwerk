// <copyright file="Heartbeat.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PiStellwerk.Data;

namespace PiStellwerkLoadGenerator.ClientActions
{
    /// <summary>
    /// The regular Heartbeat sent by a client to ensure a working connection to the server.
    /// </summary>
    [UsedImplicitly]
    public class Heartbeat : ClientActionBase
    {
        private User _user;

        /// <inheritdoc/>
        public override int Interval => Random.Next(1900, 2100);

        /// <inheritdoc />
        public override Task Initialize(HttpClient client, Options options, Random random)
        {
            _user = new User()
            {
                Name = random.Next(1, 999999999).ToString(),
                UserAgent = "PiStellwerk ",
            };

            return base.Initialize(client, options, random);
        }

        /// <inheritdoc/>
        public override async Task<int> PerformRequest()
        {
            var startTime = DateTime.Now;
            var content = new StringContent(JsonSerializer.Serialize(_user), Encoding.UTF8, "application/json");
            _ = await Client.PutAsync(Options.Uri + "status", content);
            return (int)Math.Round((DateTime.Now - startTime).TotalMilliseconds);
        }
    }
}
