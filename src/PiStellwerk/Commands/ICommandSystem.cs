// <copyright file="ICommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Threading.Tasks;
using PiStellwerk.Database;
using PiStellwerk.Database.Model;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// Interface for implementing communication with a specific command system.
    /// </summary>
    public interface ICommandSystem
    {
        public delegate void StatusChangeHandler(bool isRunning);

        public event StatusChangeHandler StatusChanged;

        public Task HandleSystemStatus(bool shouldBeRunning);

        public Task HandleEngineSpeed(Engine engine, short speed, bool? forward);

        public Task HandleEngineEStop(Engine engine);

        public Task HandleEngineFunction(Engine engine, byte functionNumber, bool on);

        /// <summary>
        /// Load engines from the command system. Will do nothing if the system doesn't know about engines.
        /// </summary>
        /// <param name="context"><see cref="PiStellwerk.Database.StwDbContext"/> to compare/insert engines.</param>
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
        public virtual Task<bool> TryAcquireEngine(Engine engine)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// "Release" an engine.
        /// <see cref="TryAcquireEngine"/> to "acquire" an engine again.
        /// </summary>
        /// <param name="engine">The engine to release.</param>
        /// <returns>A bool indicating whether the operation was successful.</returns>
        public virtual Task<bool> TryReleaseEngine(Engine engine)
        {
            return Task.FromResult(true);
        }
    }
}
