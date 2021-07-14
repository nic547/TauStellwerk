// <copyright file="EngineDtoGenerator.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using PiStellwerk.Base.Model;
using PiStellwerk.Util;

namespace PiStellwerk.Test
{
    public static class EngineDtoGenerator
    {
        private static readonly List<EngineNameSet> _engineNameComponents = new()
        {
            new("BLS Re 465", 1, 18, 160, new() { "Universal", "SLM" }),
            new("BLS Re 475", 1, 15, 200, new() { "Freight", "Siemens" }),
            new("BLS Re 485", 1, 20, 140, new() { "Freight", "Bombardier" }),
            new("BLS Re 486", 1, 10, 140, new() { "Freight", "Bombardier" }),

            new("SBB Re 420", 101, 349, 140, new() { "Universal", "SLM" }),
            new("SBB Re 620", 0, 89, 140, new() { "Universal", "SLM" }),
            new("SBB Re 460", 0, 118, 200, new() { "Passenger", "SLM" }),
            new("SBB Re 474", 1, 18, 140, new() { "Freight", "Siemens" }),
            new("SBB Re 482", 0, 49, 140, new() { "Freight", "Bombardier" }),
            new("SBB Re 484", 2, 21, 140, new() { "Freight", "Bombardier" }),
            new("SBB Re 484", 103, 105, 140, new() { "Freight", "Bombardier" }),
            new("SBB Ae 610", 401, 520, 125, new() { "Universal", "SLM" }),

            new("SOB Re 446", 15, 18, 160, new() { "Universal", "SLM" }),
            new("SOB Re 456", 91, 96, 130, new() { "Universal", "SLM" }),

            new("SRT Rem 487", 1, 1, 140, new() { "Freight", "Bombardier" }),

            new("Railcare Rem 476", 451, 457, 200, new() { "Freight", "Siemens" }),
        };

        private static readonly List<string> _manufacturerTags = new() { "Märklin", "Pikp", "Roco", "LS Models", "HAG", "Lilliput", "ESU" };
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
            var totalPossibleEngineNames = _engineNameComponents.Sum(nameSet => nameSet.MaxNumber - nameSet.MinNumber);
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
                Name = engineNameSet.ClassName + " " + number.ToString("000"),
                Address = (ushort)random.Next(ushort.MinValue, ushort.MaxValue),
                TopSpeed = engineNameSet.TopSpeed,
                Tags = tags,
                LastUsed = new DateTime(2021, 1, 1).AddDays(random.Next(0, 364)).AddHours(random.Next(0, 23)).AddMinutes(random.Next(0, 59)).AddSeconds(random.Next(0, 59)),
                Created = new DateTime(2020, 1, 1).AddDays(random.Next(0, 364)).AddHours(random.Next(0, 23)).AddMinutes(random.Next(0, 59)).AddSeconds(random.Next(0, 59)),
                Functions = new()
                {
                    new(0, "Headlights"),
                    new(1, "Sound"),
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

            public int TotalNumber => MaxNumber - MinNumber;

            public int TopSpeed { get; }

            public List<string> Tags { get; }
        }
    }
}