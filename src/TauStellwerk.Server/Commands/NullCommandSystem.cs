// <copyright file="NullCommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Commands
{
    /// <summary>
    /// Implements a <see cref="CommandSystemBase"/> that does nothing.
    /// </summary>
    public class NullCommandSystem : CommandSystemBase
    {
        public NullCommandSystem(IConfiguration config)
            : base(config)
        {
        }

        /// <inheritdoc/>
        public override Task HandleEngineEStop(Engine engine, bool hasBeenDrivingForwards)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task HandleEngineFunction(Engine engine, byte functionNumber, bool on)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task HandleSystemStatus(bool shouldBeRunning)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task HandleEngineSpeed(Engine engine, short speed, bool hasBeenDrivingForward, bool shouldBeDrivingForward)
        {
            return Task.CompletedTask;
        }
    }
}
