// <copyright file="CommandSystemFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TauStellwerk.Commands.ECoS;

namespace TauStellwerk.Commands;

/// <summary>
/// Contains a factory for instances that implement ICommandSystem.
/// </summary>
public static class CommandSystemFactory
{
    private static readonly List<(Type Type, Func<IConfiguration, ILogger<CommandSystemBase>, CommandSystemBase> Constructor)> _commandStations = new()
    {
        (typeof(NullCommandSystem), (config, _) => new NullCommandSystem(config)),
        (typeof(ConsoleCommandSystem), (config, _) => new ConsoleCommandSystem(config)),
        (typeof(EsuCommandStation), (config, logger) => new EsuCommandStation(config, logger)),
        (typeof(DccExSerialSystem), (config, logger) => new DccExSerialSystem(config, logger)),
    };

    public static CommandSystemBase FromConfig(IConfiguration config, ILogger<CommandSystemBase> logger)
    {
        var systemSetting = config["CommandSystem:Type"];

        foreach (var system in _commandStations)
        {
            if (string.Equals(systemSetting, system.Type.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return system.Constructor.Invoke(config, logger);
            }
        }

        logger.LogError($"Could not find CommandSystem \"{systemSetting}\", continuing with default (ConsoleCommandSystem)");
        return new ConsoleCommandSystem(config);
    }
}