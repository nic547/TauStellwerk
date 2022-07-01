// <copyright file="CommandSystemFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TauStellwerk.Util;

namespace TauStellwerk.Server.CommandStation;

/// <summary>
/// Contains a factory for instances that implement ICommandSystem.
/// </summary>
public static class CommandSystemFactory
{
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident", Justification = "Type is clearly visible.")]
    private static readonly List<CommandStationEntry> _commandStation = new()
    {
        new(typeof(NullCommandSystem), (config, _) => new NullCommandSystem(config)),
        new(typeof(ConsoleCommandSystem), (config, _) => new ConsoleCommandSystem(config)),
        new(typeof(ECoSCommandStation), (config, logger) => new ECoSCommandStation(config, logger)),
        new(typeof(DccExSerialCommandStation), (config, logger) => new DccExSerialCommandStation(config, logger)),
    };

    public static CommandSystemBase FromConfig(IConfiguration config, ILogger<CommandSystemBase> logger)
    {
        var systemSetting = config["CommandSystem:Type"];

        CommandStationEntry? bestMatch = null;
        var bestMatchDistance = int.MaxValue;

        foreach (var system in _commandStation)
        {
            var distance = OptimalStringAlignment.Calculate(systemSetting.ToLowerInvariant(), system.Name.ToLowerInvariant());

            if (distance == 0)
            {
                return system.Constructor.Invoke(config, logger);
            }

            if (distance < bestMatchDistance)
            {
                bestMatch = system;
                bestMatchDistance = distance;
            }
        }

        if (bestMatch is not null && bestMatchDistance < bestMatch.Name.Length / 2)
        {
            logger.LogWarning("Couldn't locate an Implementation for CommandSystem {systemSetting}, continuing with similar system {bestMatch}", systemSetting, bestMatch.Name);
            return bestMatch.Constructor.Invoke(config, logger);
        }

        logger.LogError("Could not find CommandSystem \"{systemSetting}\", continuing with default (NullCommandSystem)", systemSetting);
        return new NullCommandSystem(config);
        }

    private record CommandStationEntry
    {
        public CommandStationEntry(Type type, Func<IConfiguration, ILogger<CommandSystemBase>, CommandSystemBase> constructor)
        {
            Name = GetHumanFriendlyName(type);
            Constructor = constructor;
        }

        public string Name { get; }

        public Func<IConfiguration, ILogger<CommandSystemBase>, CommandSystemBase> Constructor { get; init; }

    }

    private static string GetHumanFriendlyName(Type commandSystemType)
    {
        var name = commandSystemType.Name;

        if (name.EndsWith("CommandSystem"))
        {
            // Remove suffix from string
            return name[..^13];
        }

        return name;
    }
}