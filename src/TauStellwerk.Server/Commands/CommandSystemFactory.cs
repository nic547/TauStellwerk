// <copyright file="CommandSystemFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TauStellwerk.Commands.ECoS;
using TauStellwerk.Util;

namespace TauStellwerk.Commands;

/// <summary>
/// Contains a factory for instances that implement ICommandSystem.
/// </summary>
public static class CommandSystemFactory
{
    private static readonly List<(Type Type, Func<IConfiguration, CommandSystemBase> Constructor)> _commandStations = new()
    {
        (typeof(NullCommandSystem), (x) => new NullCommandSystem(x)),
        (typeof(ConsoleCommandSystem), (x) => new ConsoleCommandSystem(x)),
        (typeof(EsuCommandStation), (x) => new EsuCommandStation(x)),
        (typeof(DccExSerialSystem), (x) => new DccExSerialSystem(x)),
    };

    /// <summary>
    /// Create a instance of a class that implements <see cref="CommandSystemBase"/>.
    /// </summary>
    /// <param name="config">Config that might contain a setting for the CommandSystemBase.</param>
    /// <returns>A CommandSystemBase. Default is the ConsoleCommandSystem.</returns>
    public static CommandSystemBase FromConfig(IConfiguration config)
    {
        var systemSetting = config["CommandSystem:Type"];

        foreach (var system in _commandStations)
        {
            if (string.Equals(systemSetting, system.Type.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return system.Constructor.Invoke(config);
            }
        }

        ConsoleService.PrintError($"Could not find CommandSystem \"{systemSetting}\", continuing with default (ConsoleCommandSystem)");
        return new ConsoleCommandSystem(config);
    }
}