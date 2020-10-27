// <copyright file="CommandType.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data.Commands
{
    /// <summary>
    /// Designates the what type of command a command is.
    /// </summary>
    public enum CommandType : byte
    {
        /// <summary>
        /// Designates a command that relates to speed.
        /// </summary>
        Speed,

        /// <summary>
        /// Designates a command that toggles a function on.
        /// </summary>
        FunctionToggleOn,

        /// <summary>
        /// Designates a command that toggles a function off.
        /// </summary>
        FunctionToggleOff,

        /// <summary>
        /// Designates a command that turns a momentary function on.
        /// </summary>
        FunctionOn,
    }
}