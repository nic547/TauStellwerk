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
    public static class Heartbeat
    {
        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <param name="options"><see cref="Options"/>.</param>
        /// <param name="user">The <see cref="User"/> the request shall be made for.</param>
        /// <param name="client"><see cref="HttpClient"/> with which the request shall be made.</param>
        /// <returns>Measured latency in 1/10ms.</returns>
        public static async Task<int> PerformRequest(Options options, User user, HttpClient client)
        {
            var startTime = DateTime.Now;
            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            _ = await client.PutAsync(options.Uri + "status", content);

            return (int)Math.Round((DateTime.Now - startTime).TotalMilliseconds * 10d);
        }
    }
}
