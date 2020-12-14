// <copyright file="ConsoleCommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Web;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// <see cref="ICommandSystem"/> that just writes everything to the console.
    /// </summary>
    public class ConsoleCommandSystem : ICommandSystem
    {
        /// <inheritdoc/>
        public void HandleCommand(JsonCommand command, Engine engine)
        {
            switch (command.Type)
            {
                case CommandType.Speed:
                    Console.WriteLine($"New speed for engine \"{HttpUtility.HtmlDecode(engine.Name)}\": {command.Data}");
                    break;
                case CommandType.FunctionToggleOn:
                case CommandType.FunctionToggleOff:
                case CommandType.FunctionOn:
                    Console.WriteLine($"{HttpUtility.HtmlDecode(engine.Name)} got a function-related command.");
                    break;
                default:
                    throw new InvalidEnumArgumentException(command.ToString());
            }
        }
    }
}
