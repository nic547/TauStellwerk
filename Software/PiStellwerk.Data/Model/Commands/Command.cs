// <copyright file="Command.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data.Commands
{
    /// <summary>
    /// Represents a command. Mostly for the server to keep track of commands.
    /// Use <see cref="JsonCommand"/> for sending stuff to the WebAPI.
    /// </summary>
    public class Command : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="address">DCC Address of the engine.</param>
        /// <param name="speedSteps">How many speed steps the decoder of the engine supports.</param>
        /// <param name="data">Data the command gets. Depends on type.</param>
        /// <param name="type">The type of command.</param>
        public Command(ushort address, byte speedSteps, byte data, CommandType type)
        {
            Address = address;
            SpeedSteps = speedSteps;
            Data = data;
            Type = type;
        }

        /// <summary>
        /// Gets the DCC address of the engine this command is meant for.
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Gets how many speedSteps the decoder of the engine supports.
        /// </summary>
        public byte SpeedSteps { get; }
    }
}
