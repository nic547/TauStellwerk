// <copyright file="JsonContextRoundTripTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Base.Tests;

public class JsonContextRoundTripTests
{
    [TestCase(Direction.Forwards)]
    [TestCase(Direction.Backwards)]
    [TestCase(SortEnginesBy.Created)]
    [TestCase(SortEnginesBy.LastUsed)]
    [TestCase(SortEnginesBy.Name)]
    [TestCase(State.On)]
    [TestCase(State.Off)]
    [TestCase(TurnoutKind.RightTurnout)] // TurnoutKind not tested exhaustively
    [TestCase(TurnoutKind.SlimCurvedLeftTurnout)]
    [TestCase(TurnoutKind.SlimCurvedRightTurnout)]
    public void EnumTests<T>(T value)
    where T : Enum
    {
        var json = JsonSerializer.Serialize(value, typeof(T), TauJsonContext.Default);
        var result = JsonSerializer.Deserialize(json, typeof(T), TauJsonContext.Default);

        result.Should().Be(value);
    }

    [Test]
    public void EngineOverviewDtoTest()
    {
        var dto = new EngineOverviewDto()
        {
            Id = 3,
            Name = """SBB Re 482 ("Alpäzähmer") """,
            Tags = new() { "Sound", "DCC", "91 85 4482 026-2 CH-SBBC", ",", ""","",""" },
            Created = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastUsed = DateTime.UtcNow,
            Images = new()
            {
                new ImageDto("3_100.jpg", 800),
                new ImageDto("3_100.webp", 800),
                new ImageDto("3_100.avif", 800),
                new ImageDto("3_50.jpg", 400),
                new ImageDto("3_50.webp", 400),
                new ImageDto("3_50.avif", 400),
                new ImageDto("3_25.jpg", 200),
                new ImageDto("3_25.webp", 200),
                new ImageDto("3_25.avif", 200),
            },
        };

        var json = JsonSerializer.Serialize(dto, TauJsonContext.Default.EngineOverviewDto);
        var result = JsonSerializer.Deserialize(json, TauJsonContext.Default.EngineOverviewDto);
        result.Should().BeEquivalentTo(dto);
    }

    [Test]
    public void EngineFullDtoTest()
    {
        var dto = new EngineFullDto()
        {
            Id = 4,
            Name = """SBB Re 482 ("Alpäzähmer") """,
            Tags = new() { "Sound", "DCC", "91 85 4482 026-2 CH-SBBC" },
            Created = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastUsed = DateTime.UtcNow,
            Images = new()
            {
                new ImageDto("3_100.jpg", 800),
                new ImageDto("3_100.webp", 800),
                new ImageDto("3_100.avif", 800),
                new ImageDto("3_50.jpg", 400),
                new ImageDto("3_50.webp", 400),
                new ImageDto("3_50.avif", 400),
                new ImageDto("3_25.jpg", 200),
                new ImageDto("3_25.webp", 200),
                new ImageDto("3_25.avif", 200),
            },
            Functions = new()
            {
                new(0, "Light", 0) { State = State.On },
                new(1, "Sound", 0) { State = State.Off },
            },
            Address = 26,
            TopSpeed = 140,
            Throttle = 100,
            Direction = Direction.Forwards,
        };

        var json = JsonSerializer.Serialize(dto, TauJsonContext.Default.EngineFullDto);
        var result = JsonSerializer.Deserialize(json, TauJsonContext.Default.EngineFullDto);
        result.Should().BeEquivalentTo(dto);
    }

    [Test]
    public void TurnoutDtoTest()
    {
        var dto = new TurnoutDto()
        {
            Id = 1,
            Address = 1,
            IsInverted = true,
            Kind = TurnoutKind.RightTurnout,
            Name = "W1 BHF XYZ",
            State = State.On,
        };

        var json = JsonSerializer.Serialize(dto, TauJsonContext.Default.TurnoutDto);
        var result = JsonSerializer.Deserialize(json, TauJsonContext.Default.TurnoutDto);

        result.Should().BeEquivalentTo(dto);
    }
}