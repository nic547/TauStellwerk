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
    public record Command : CommandBase
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
        public ushort Address { get; init; }

        /// <summary>
        /// Gets how many speedSteps the decoder of the engine supports.
        /// </summary>
        public byte SpeedSteps { get; }

        /// <summary>
        /// Gets a command that ends up as a DCC Idle Packet.
        /// </summary>
        public static Command IdleCommand { get; } = new(0, 128, byte.MaxValue, CommandType.Speed);

        /// <summary>
        /// Check if this command can replaced by a given command.
        /// Example: A command telling a train to drive 81 is replaced by a subsequent command telling the train to drive 82.
        /// </summary>
        /// <param name="command2">The given command.</param>
        /// <returns>Bool indicating if this command can be replaced by the given command.</returns>
        public bool IsReplaceableBy(Command command2)
        {
            if (Address != command2.Address)
            {
                return false;
            }

            switch (Type, command2.Type)
            {
                case (CommandType.Speed, CommandType.Speed):
                    return SpeedSteps == command2.SpeedSteps;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if a command negates another command.
        /// Example: Turning on headlights and turning them off again.
        /// </summary>
        /// <param name="command2">The command to check against.</param>
        /// <returns>Bool indicating whether this command is negated by the supplied command.</returns>
        public bool IsNegatedBy(Command command2)
        {
            if (Address != command2.Address)
            {
                return false;
            }

            switch (Type, command2.Type)
            {
                case (CommandType.FunctionToggleOff, CommandType.FunctionToggleOn):
                case (CommandType.FunctionToggleOn, CommandType.FunctionToggleOff):
                    return Data == command2.Data;

                default:
                    return false;
            }
        }
    }
}
