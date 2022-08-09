// <copyright file="EngineDtoGenerator.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Base;
using TauStellwerk.Util;

namespace TauStellwerk.Tools.CreateTestDb;

public static class EngineDtoGenerator
{
    private static readonly List<EngineNameSet> _engineNameComponents = new()
    {
        new("BLS Re 465 {0:000} (Blue)", 1, 18, 160, new() { "Universal", "SLM" }),
        new("BLS Re 465 {0:000} (Green)", 1, 18, 160, new() { "Universal", "SLM" }),
        new("BLS Re 475 {0:000} (Alpinist)", 1, 15, 200, new() { "Freight", "Siemens" }),
        new("BLS Re 485 {0:000} (Connecting Europe)", 1, 20, 140, new() { "Freight", "Bombardier" }),
        new("BLS Re 485 {0:000} (Alpinist)", 1, 20, 140, new() { "Freight", "Bombardier" }),
        new("BLS Re 486 {0:000} (Alpinist)", 1, 10, 140, new() { "Freight", "Bombardier" }),

        new("Hupac BR 193 {0:000}", 490, 497, 200, new() { "Freight", "Siemens", "DACHINL", "Vectron MS" }),

        new("Railcare Rem 476 {0:000}", 451, 457, 200, new() { "Freight", "Siemens" }),

        new("SBB Re 4/4 {0} (Green)", 11101, 11376, 140, new() { "Universal", "SLM" }),
        new("SBB Re 420 {0:000} (Red)", 101, 349, 140, new() { "Universal", "SLM" }),
        new("SBB Re 420 {0:000} (LION)", 201, 230, 140, new() { "Universal", "SLM" }),
        new("SBB Re 620 {0:000}", 0, 89, 140, new() { "Universal", "SLM" }),
        new("SBB Re 460 {0:000} (Lok 2000)", 0, 118, 200, new() { "Passenger", "SLM" }),
        new("SBB Re 460 {0:000} (Refit)", 0, 118, 200, new() { "Passenger", "SLM" }),
        new("SBB Re 474 {0:000} (SBB Cargo)", 1, 18, 140, new() { "Freight", "Siemens" }),
        new("SBB Re 482 {0:000} (SBB Cargo)", 0, 49, 140, new() { "Freight", "Bombardier" }),
        new("SBB Re 484 {0:000} (SBB Cargo)", 2, 21, 140, new() { "Freight", "Bombardier" }),
        new("SBB Re 484 {0:000} (SBB Cargo)", 103, 105, 140, new() { "Freight", "Bombardier" }),
        new("SBB Ae 610 {0:000} (Red)", 401, 520, 125, new() { "Universal", "SLM" }),
        new("SBB BR 193 {0:000} (Daypiercer)", 461, 478, 20, new() { "Freight", "Siemens", "LokRoll" }),

        new("SOB Re 446 {0:000}", 15, 18, 160, new() { "Universal", "SLM" }),
        new("SOB Re 456 {0:000}", 91, 96, 130, new() { "Universal", "SLM" }),

        new("SRT Rem 487 {0:000} (Biene Maja)", 1, 1, 140, new() { "Freight", "Bombardier" }),
    };

    private static readonly List<string> _manufacturerTags = new() { "Märklin", "Piko", "Roco", "LS Models", "HAG", "Lilliput", "ESU" };
    private static readonly List<string> _ownerTag = new() { "Peter", "Mike", "Sara", "Max", "Jean" };

    public static EngineFullDto GetEngineDto()
    {
        return CreateEngine(new Random());
    }

    public static IList<EngineFullDto> GetEngineFullDtos(int number)
    {
        if (number < 0)
        {
            throw new ArgumentException("Cannot create a negative number of engines", nameof(number));
        }

        var random = new Random();
        var engines = new List<EngineFullDto>();

        for (var i = 0; i < number; i++)
        {
            engines.Add(CreateEngine(random));
        }

        return engines;
    }

    private static EngineFullDto CreateEngine(Random random)
    {
        var totalPossibleEngineNames = _engineNameComponents.Sum(nameSet => nameSet.TotalNumber);
        var chosenIndex = random.Next(0, totalPossibleEngineNames);

        EngineNameSet? engineNameSet = null;
        var number = 0;

        foreach (var currentSet in _engineNameComponents)
        {
            if (chosenIndex < currentSet.TotalNumber)
            {
                number = currentSet.MinNumber + chosenIndex;
                engineNameSet = currentSet;
                break;
            }

            chosenIndex -= currentSet.TotalNumber;
        }

        if (engineNameSet is null)
        {
            throw new InvalidOperationException();
        }

        var tags = new List<string>();
        tags.AddRange(engineNameSet.Tags);
        tags.Add(_manufacturerTags.TakeRandom(random));
        tags.Add(_ownerTag.TakeRandom(random));

        return new EngineFullDto()
        {
            Name = string.Format(engineNameSet.ClassName, number),
            Address = (ushort)random.Next(ushort.MinValue, ushort.MaxValue),
            TopSpeed = engineNameSet.TopSpeed,
            Tags = tags,
            LastUsed = new DateTime(2021, 1, 1).AddDays(random.Next(0, 364)).AddHours(random.Next(0, 23)).AddMinutes(random.Next(0, 59)).AddSeconds(random.Next(0, 59)),
            Created = new DateTime(2020, 1, 1).AddDays(random.Next(0, 364)).AddHours(random.Next(0, 23)).AddMinutes(random.Next(0, 59)).AddSeconds(random.Next(0, 59)),
            Functions = new()
            {
                new(0, "Headlights", 0),
                new(1, "Sound", 0),
            },
        };
    }

    private record EngineNameSet
    {
        public EngineNameSet(string className, int minNumber, int maxNumber, int topSpeed, List<string> tags)
        {
            ClassName = className;
            MinNumber = minNumber;
            MaxNumber = maxNumber;
            TopSpeed = topSpeed;
            Tags = tags;
        }

        public string ClassName { get; }

        public int MinNumber { get; }

        public int MaxNumber { get; }

        public int TotalNumber => MaxNumber - MinNumber + 1;

        public int TopSpeed { get; }

        public List<string> Tags { get; }
    }
}
