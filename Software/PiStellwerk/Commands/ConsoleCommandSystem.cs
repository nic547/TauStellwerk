// <copyright file="ConsoleCommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using PiStellwerk.Data;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// <see cref="ICommandSystem"/> that just writes everything to the console.
    /// </summary>
    public class ConsoleCommandSystem : ICommandSystem
    {
        public Task HandleEngineEStop(Engine engine)
        {
            throw new NotImplementedException();
        }

        public Task HandleEngineFunction(Engine engine, byte functionNumber, bool on)
        {
            throw new NotImplementedException();
        }

        public Task HandleEngineSpeed(Engine engine, short speed, bool? forward)
        {
            throw new NotImplementedException();
        }

        public Task HandleSystemStatus(bool shouldBeRunning)
        {
            throw new NotImplementedException();
        }
    }
}
