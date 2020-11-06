// <copyright file="JsonCommand.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data.Commands
{
    /// <summary>
    /// A command for an engine, meant for JSON and communication between server/clients.
    /// These commands do not know about dcc addresses or ids of engines.
    /// </summary>
    public class JsonCommand : CommandBase
    {
        /// <summary>
        /// Turn the JSONCommand into a Command by adding address and speedSteps of the engine.
        /// </summary>
        /// <param name="address">The DCC address of the engine.</param>
        /// <param name="speedSteps">The speedSteps the decoder of the engine supports.</param>
        /// <returns>A <see cref="Command"/>.</returns>
        public Command ToCommand(ushort address, byte speedSteps)
        {
            return new Command(address, speedSteps, Data, Type);
        }
    }
}
