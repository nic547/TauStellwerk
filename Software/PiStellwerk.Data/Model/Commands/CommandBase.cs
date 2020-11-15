// <copyright file="CommandBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data.Commands
{
    /// <summary>
    /// Abstract base class for commands.
    /// </summary>
    public abstract record CommandBase
    {
        /// <summary>
        /// Gets the type of this command.
        /// </summary>
        public CommandType Type { get; init; }

        /// <summary>
        /// Gets or sets data of this command. Depending on the type of command, this might be the "number" of function or a speed.
        /// </summary>
        public byte Data { get; set; }
    }
}
