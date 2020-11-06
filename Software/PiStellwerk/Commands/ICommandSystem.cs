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
    }
}
