// <copyright file="ConsoleCommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PiStellwerk.Database.Model;
using PiStellwerk.Util;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// <see cref="ICommandSystem"/> that just writes everything to the console.
    /// </summary>
    public class ConsoleCommandSystem : ICommandSystem
    {
        // ReSharper disable once UnusedParameter.Local
        public ConsoleCommandSystem(IConfiguration config)
        {
        }

        public event ICommandSystem.StatusChangeHandler StatusChanged
        {
            add { }
            remove { }
        }

        public Task HandleEngineEStop(Engine engine)
        {
            ConsoleService.PrintMessage($"{engine} has been emergency-stopped");
            return Task.CompletedTask;
        }

        public Task HandleEngineFunction(Engine engine, byte functionNumber, bool on)
        {
            ConsoleService.PrintMessage($"{engine} function {functionNumber} has been turned {(on ? "on" : "off")}");
            return Task.CompletedTask;
        }

        public Task HandleEngineSpeed(Engine engine, short speed, bool? forward)
        {
            if (forward != null)
            {
                var forwardValue = (bool)forward;
                ConsoleService.PrintMessage($"{engine} speed  has been set to {speed}, driving {(forwardValue ? "forward" : "backwards")}");
            }
            else
            {
                ConsoleService.PrintMessage($"{engine} speed  has been set to {speed}");
            }

            return Task.CompletedTask;
        }

        public Task HandleSystemStatus(bool shouldBeRunning)
        {
            ConsoleService.PrintMessage($"System was {(shouldBeRunning ? "started" : "stopped")}");
            return Task.CompletedTask;
        }
    }
}
