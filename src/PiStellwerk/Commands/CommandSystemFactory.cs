// <copyright file="CommandSystemFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// Contains a factory for instances that implement ICommandSystem.
    /// </summary>
    public static class CommandSystemFactory
    {
        /// <summary>
        /// Create a instance of a class that implements <see cref="ICommandSystem"/>.
        /// </summary>
        /// <param name="config">Config that might contain a setting for the CommandSystem.</param>
        /// <returns>A CommandSystem. Default is the ConsoleCommandSystem.</returns>
        public static ICommandSystem FromConfig(IConfiguration config)
        {
            var setting = config["CommandSystem:Type"];
            var systems = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommandSystem).IsAssignableFrom(p) && !p.IsInterface)
                .ToImmutableList();
            foreach (var system in systems)
            {
                if (setting == system.Name)
                {
                     var systemInstance = Activator.CreateInstance(system, config) as ICommandSystem;
                     return systemInstance ?? new ConsoleCommandSystem(config);
                }
            }

            return new ConsoleCommandSystem(config);
        }
    }
}
