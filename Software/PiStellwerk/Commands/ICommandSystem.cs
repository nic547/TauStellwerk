// <copyright file="ICommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Threading.Tasks;
using PiStellwerk.Data;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// Interface for implementing communication with a specific command system.
    /// </summary>
    public interface ICommandSystem
    {
        /// <summary>
        /// Handle a Command that relates to the status of the commandSystem (running or not).
        /// </summary>
        /// <param name="shouldBeRunning">A value indicating whether the system should be started or stopped.</param>
        public void HandleSystemStatus(bool shouldBeRunning)
        {
        }

        public Task HandleEngineSpeed(Engine engine, short speed, bool? forward);

        public Task HandleEngineEStop(Engine engine);

        public Task HandleEngineFunction(Engine engine, byte functionNumber, bool on);

        /// <summary>
        /// Check if the CommandSystem is in a running state.
        /// </summary>
        /// <returns>A bool indicating whether the commandSystem is in a running state.</returns>
        public virtual Task<bool?> CheckStatusAsync()
        {
            return Task.FromResult<bool?>(null);
        }

        /// <summary>
        /// Load engines from the command system. Will do nothing if the system doesn't know about engines.
        /// </summary>
        /// <param name="context"><see cref="StwDbContext"/> to compare/insert engines.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task LoadEnginesFromSystem(StwDbContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// "Acquire" an engine - Some command system may require explicitly taking control of a engine before sending commands.
        /// <see cref="TryReleaseEngine"/> to "release" an engine again.
        /// </summary>
        /// <param name="engine">The engine to acquire.</param>
        /// <returns>A bool indicating whether the operation was successful.</returns>
        public virtual bool TryAcquireEngine(Engine engine)
        {
            return true;
        }

        /// <summary>
        /// "Release" an engine.
        /// <see cref="TryAcquireEngine"/> to "acquire" an engine again.
        /// </summary>
        /// <param name="engine">The engine to release.</param>
        /// <returns>A bool indicating whether the operation was successful.</returns>
        public virtual bool TryReleaseEngine(Engine engine)
        {
            return true;
        }
    }
}
