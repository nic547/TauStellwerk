// <copyright file="ICommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// Anything that takes <see cref="JsonCommand"/>s, turns them into another representation and outputs it via SPI/USB/Console etc.
    /// </summary>
    public interface ICommandSystem
    {
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="engine">The Engine this command was sent for.</param>
        public void HandleCommand(JsonCommand command, Engine engine);

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
