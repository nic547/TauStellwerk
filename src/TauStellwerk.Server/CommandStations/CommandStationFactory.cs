// <copyright file="CommandStationFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TauStellwerk.Util;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// Contains a factory for instances that implement ICommandSystem.
/// </summary>
public static class CommandStationFactory
{
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident", Justification = "Type is clearly visible.")]
    private static readonly List<CommandStationEntry> _commandStations = new()
    {
        new(typeof(NullCommandStation), (config, _) => new NullCommandStation(config)),
        new(typeof(ConsoleCommandStation), (config, _) => new ConsoleCommandStation(config)),
        new(typeof(ECoSCommandStation), (config, logger) => new ECoSCommandStation(config, logger)),
        new(typeof(DccExSerialCommandStation), (config, logger) => new DccExSerialCommandStation(config, logger)),
    };

    public static CommandStationBase FromConfig(IConfiguration config, ILogger<CommandStationBase> logger)
    {
        var systemSetting = config["CommandStation:Type"];

        CommandStationEntry? bestMatch = null;
        var bestMatchDistance = int.MaxValue;

        foreach (var system in _commandStations)
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
            logger.LogWarning("Couldn't locate an Implementation for command station \"{systemSetting}\", continuing with similar implementation: \"{bestMatch}\"", systemSetting, bestMatch.Name);
            return bestMatch.Constructor.Invoke(config, logger);
        }

        logger.LogError("Could not find an implementation for command station \"{systemSetting}\", continuing with default (NullCommandStation)", systemSetting);
        return new NullCommandStation(config);
        }

    private static string GetHumanFriendlyName(Type commandSystemType)
    {
        var name = commandSystemType.Name;

        if (name.EndsWith("CommandStation"))
        {
            // Remove suffix from string
            return name[..^14];
        }

        return name;
    }

    private record CommandStationEntry
    {
        public CommandStationEntry(Type type, Func<IConfiguration, ILogger<CommandStationBase>, CommandStationBase> constructor)
        {
            Name = GetHumanFriendlyName(type);
            Constructor = constructor;
        }

        public string Name { get; }

        public Func<IConfiguration, ILogger<CommandStationBase>, CommandStationBase> Constructor { get; init; }
    }
}