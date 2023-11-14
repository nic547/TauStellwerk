// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Dto;
using TauStellwerk.Util;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace TauStellwerk.Tools.CreateTestDb;

public static class EngineDtoGenerator
{
    private static readonly List<EngineNameSet> _engineNameComponents = new()
    {
        new("BLS Re 465 {0:000} (Blue)", 1, 18, 160, new() { "Universal", "Long-Distance", "Regional", "SLM", "CH" }),
        new("BLS Re 465 {0:000} (Green)", 1, 18, 160, new() { "Universal", "Long-Distance", "Regional", "SLM", "CH" }),
        new("BLS Re 475 {0:000} (Alpinist)", 401, 415, 200, new() { "Freight", "Siemens", "D/A/CH/I/NL", "Vectron MS" }),
        new("BLS Re 475 {0:000} (Alpinist)", 416, 440, 200, new() { "Freight", "Siemens", "D/A/CH/I/NL/B", "Vectron MS" }),
        new("BLS Re 485 {0:000} (Connecting Europe)", 1, 20, 140, new() { "Freight", "Bombardier", "D/CH", "Traxx F140 AC1" }),
        new("BLS Re 485 {0:000} (Alpinist)", 1, 20, 140, new() { "Freight", "Bombardier", "D/CH", "Traxx F140 AC1" }),
        new("BLS Re 486 {0:000} (Alpinist)", 1, 10, 140, new() { "Freight", "Bombardier", "D/A/CH/I", "Traxx F140 MS2E" }),

        new("DB BR 185 {0:000}", 85, 94, 140, new() { "Freight", "Bombardier", "Traxx F140 AC1", "D/A/CH" }),
        new("DB BR 185 {0:000}", 95, 149, 140, new() { "Freight", "Bombardier", "Traxx F140 AC1", "D/CH" }),

        new("Hupac BR 193 {0:000}", 490, 497, 200, new() { "Freight", "Siemens", "D/A/CH/I/NL", "Vectron MS" }),

        new("Railcare Rem 476 {0:000}", 451, 457, 200, new() { "Freight", "Siemens", "Vectron AC" }),

        new("SBB Ae 6/6 {0} (Green)", 11401, 11520, 125, new() { "SLM", "BBC", "MFO", "Universal", "Long-Distance", "Regional", "CH" }),
        new("SBB Ae 6/6 {0} (Red)", 11401, 11520, 125, new() { "SLM", "BBC", "MFO", "Universal", "Long-Distance", "Regional", "CH" }),
        new("SBB Ae 610 {0:000} (Red)", 401, 520, 125, new() { "Universal", "Long-Distance", "Regional", "SLM", "CH" }),
        new("SBB RABDe 500 {0:000}", 0, 43, 200, new() { "Passenger", "Long-Distance", "ICN", "Adtranz", "Tilting Train", "CH" }),
        new("SBB RABe 501 {0:000}", 1, 29, 250, new() { "Giruno", "Long-Distance", "Passenger", "Stadler", "D/A/CH/I" }),
        new("SBB RBDe 560 {0:000}", 0, 141, 140, new() { "Passenger", "Regional", "FFA", "BBC", "Schindler Waggon", "ABB", "SIG", "CH" }),
        new("SBB RBDe 560 {0:000} (Domino)", 203, 307, 140, new() { "Passenger", "Regional", "FFA", "BBC", "Schindler Waggon", "ABB", "SIG", "CH" }),
        new("SBB RBDe 560 {0:000} (RegioAlps)", 401, 416, 140, new() { "Passenger", "Regional", "FFA", "BBC", "Schindler Waggon", "ABB", "SIG", "CH" }),
        new("SBB RBDe 560 {0:000} (Glarner Sprinter)", 201, 202, 140, new() { "Passenger", "Regional", "FFA", "BBC", "Schindler Waggon", "ABB", "SIG", "CH" }),
        new("SBB Re 4/4 {0} (Green)", 11101, 11376, 140, new() { "Universal", "Long-Distance", "Regional", "SLM", "BBC", "MFO", "SAAS", "CH" }),
        new("SBB Re 420 {0:000} (Red)", 101, 349, 140, new() { "Universal", "Long-Distance", "Regional", "SLM", "BBC", "MFO", "SAAS", "CH" }),
        new("SBB Re 420 {0:000} (LION)", 201, 230, 140, new() { "Passenger", "Regional", "SLM", "BBC", "MFO", "SAAS", "CH" }),
        new("SBB Re 6/6 {0} (Green)", 11601, 11689, 140, new() { "Universal", "Long-Distance", "SLM", "BBC", "SAAS", "CH" }),
        new("SBB Re 620 {0:000} (Red)", 0, 89, 140, new() { "Universal", "Long-Distance", "SLM", "BBC", "SAAS", "CH" }),
        new("SBB Re 450 {0:000} (DPZ)", 0, 114, 140, new() { "Passenger", "Regional", "SLM", "Schindler Waggon", "ABB", "SIG", "ZVV", "CH" }),
        new("SBB Re 450 {0:000} (DPZ+)", 0, 114, 140, new() { "Passenger", "Regional", "SLM", "Schindler Waggon", "ABB", "SIG", "ZVV", "CH" }),
        new("SBB Re 460 {0:000} (Lok 2000)", 0, 118, 200, new() { "Passenger", "Long-Distance", "SLM", "ABB", "CH" }),
        new("SBB Re 460 {0:000} (Refit)", 0, 118, 200, new() { "Passenger", "Long-Distance", "SLM", "ABB", "CH" }),
        new("SBB Re 474 {0:000} (SBB Cargo)", 1, 18, 140, new() { "Freight", "Siemens" }),
        new("SBB Re 482 {0:000} (SBB Cargo)", 0, 49, 140, new() { "Freight", "Bombardier" }),
        new("SBB Re 484 {0:000} (SBB Cargo)", 2, 21, 140, new() { "Freight", "Bombardier" }),
        new("SBB Re 484 {0:000} (SBB Cargo)", 103, 105, 140, new() { "Freight", "Bombardier" }),
        new("SBB BR 193 {0:000} (Daypiercer)", 461, 478, 20, new() { "Freight", "Siemens", "LokRoll" }),

        new("SOB RABe 526 {0:000} (FLIRT)", 41, 63, 160, new() { "Passenger", "Long-Distance", "Regional", "Stadler", "CH" }),
        new("SOB RABe 526 {0:000} (FLIRT 3)", 1, 10, 160, new() { "Passenger", "Long-Distance", "Regional", "Stadler", "CH" }),
        new("SOB RABe 526 {0:000} (Traverso)", 101, 124, 160, new() { "Passenger", "Long-Distance", "Stadler", "CH" }),
        new("SOB Re 446 {0:000}", 15, 18, 160, new() { "Universal", "Long-Distance", "SLM", "CH" }),
        new("SOB Re 456 {0:000}", 91, 96, 130, new() { "Universal", "Long-Distance", "SLM", "CH" }),

        new("SRT Rem 487 {0:000} (Biene Maja)", 1, 1, 140, new() { "Freight", "Bombardier", "D/A/CH", "Traxx AC3 (LM)" }),
    };

    private static readonly List<string> _manufacturerTags = new() { "Märklin", "Piko", "Roco", "LS Models", "HAG", "Lilliput", "ESU" };
    private static readonly List<string> _ownerTag = new() { "Peter", "Mike", "Sara", "Max", "Jean", "Ralf", "Simon", "Hans", "Ueli" };

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
        tags.AddRange(new List<string> { "DCC", "Sound" });

        // Add two random numbers as a tag to simulate the usage as "alternative names"
        // For example one could add the full UIC-Numbers to an engine => many "single-use" tags
        tags.Add(random.NextInt64(0, 999_999_999).ToString());
        tags.Add(random.NextInt64(0, 999_999_999).ToString());

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
                new(2, "Horn (short/high)", 500),
                new(3, "Horn (short/low)", 500),
                new(4, "Horn (long/high)", 0),
                new(5, "Horn (long/low)", 0),
                new(6, "High Beams", 0),
                new(7, "1x Red", 0),
                new(8, "2x Red", 0),
                new(9, "Compressor", 0),
                new(10, "Fans", 0),
                new(11, "Conductor's Whistle", 1000),
                new(12, "Shunting mode", 0),
                new(13, "Station announcement: passing train", 3500),
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
            TotalNumber = MaxNumber - MinNumber + 1;
        }

        public string ClassName { get; }

        public int MinNumber { get; }

        public int MaxNumber { get; }

        public int TotalNumber { get; }

        public int TopSpeed { get; }

        public List<string> Tags { get; }
    }
}
