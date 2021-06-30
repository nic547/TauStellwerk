// <copyright file="SpeedChange.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PiStellwerk.Client.Services;

namespace PiStellwerk.LoadGenerator.ClientActions
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
        public override async Task Initialize(ClientEngineService engineService, Options options, int id,  Random random)
        {
            await base.Initialize(engineService, options, id, random);
        }

        /// <inheritdoc/>
        public override async Task<int> PerformRequest()
        {
            int newSpeed = Random.Next(0, 128);
            var startTime = DateTime.Now;

            await EngineService.SetSpeed(Id, newSpeed, true);
            return (int)Math.Round((DateTime.Now - startTime).TotalMilliseconds);
        }
    }
}
