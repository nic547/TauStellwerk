// <copyright file="CommandStationFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Util;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// Contains a factory for instances that implement ICommandSystem.
/// </summary>
public static class CommandStationFactory
{
    private static readonly List<CommandStationEntry> _commandStations = new()
    {
        new CommandStationEntry(typeof(NullCommandStation), (_, _) => new NullCommandStation()),
        new CommandStationEntry(typeof(ConsoleCommandStation), (_, _) => new ConsoleCommandStation()),
        new CommandStationEntry(typeof(ECoSCommandStation), (config, logger) => new ECoSCommandStation(config.Get<ECoSOptions>() ?? throw new FormatException("Could not parse configuration for the ECoS"), logger)),
        new CommandStationEntry(typeof(DccExSerialCommandStation), (config, logger) => new DccExSerialCommandStation(config.Get<DccExSerialOptions>() ?? throw new FormatException("Could not parse configuration for DccExSerial"), logger)),
    };

    public static CommandStationBase FromConfig(IConfiguration config, ILogger<CommandStationBase> logger)
    {
        var systemSetting = RemoveCommandStationSuffix(config["CommandStation:Type"] ?? string.Empty);

        CommandStationEntry? bestMatch = null;
        var bestMatchDistance = int.MaxValue;

        foreach (var system in _commandStations)
        {
            var distance = OptimalStringAlignment.Calculate(systemSetting.ToLowerInvariant(), system.Name.ToLowerInvariant());

            if (distance == 0)
            {
                return system.Constructor.Invoke(config.GetSection("CommandStation"), logger);
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
            return bestMatch.Constructor.Invoke(config.GetSection("CommandStation"), logger);
        }

        logger.LogError("Could not find an implementation for command station \"{systemSetting}\", continuing with default (NullCommandStation)", systemSetting);
        return new NullCommandStation();
    }

    private static string GetHumanFriendlyName(Type commandSystemType)
    {
        var name = commandSystemType.Name;

        return RemoveCommandStationSuffix(name);
    }

    private static string RemoveCommandStationSuffix(string input)
    {
        if (input.EndsWith("CommandStation"))
        {
            // Remove suffix from string
            return input[..^14];
        }

        return input;
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