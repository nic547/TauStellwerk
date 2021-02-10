// <copyright file="SpeedChange.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PiStellwerkLoadGenerator.ClientActions
{
    /// <summary>
    /// Simulates sending a new Speed for an engine.
    /// </summary>
    [UsedImplicitly]
    public class SpeedChange : ClientActionBase
    {
        /// <inheritdoc/>
        public override int Interval => Random.Next(190, 210);

        /// <inheritdoc/>
        public override async Task Initialize(HttpClient client, Options options, Random random)
        {
            await base.Initialize(client, options, random);
        }

        /// <inheritdoc/>
        public override async Task<int> PerformRequest()
        {
            // TODO: Adopt new EngineSetSpeed functionality.
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var startTime = DateTime.Now;

            _ = await Client.PostAsync(Options.Uri + "engine/1/command/", content);
            return (int)Math.Round((DateTime.Now - startTime).TotalMilliseconds);
        }
    }
}
