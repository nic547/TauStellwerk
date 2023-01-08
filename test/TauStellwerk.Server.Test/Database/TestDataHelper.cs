// <copyright file="TestDataHelper.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Test.Database;

/// <summary>
/// Contains helpers proving sample data for testing stuff.
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Creates an engine with sample data.
    /// </summary>
    /// <returns>The engine.</returns>
    public static Engine CreateTestEngine()
    {
        return new Engine()
        {
            Name = "RE 777",
            TopSpeed = 356,
            Id = 7777,
            Address = 77,
            SpeedSteps = 128,
            Functions =
            {
                new DccFunction(0, "Lights", 0),
            },
        };
    }

    public static IReadOnlyList<Engine> CreateTestEngineList()
    {
        return new List<Engine>
        {
            new()
            {
                Name = "Roco BR 193 493 (Hupac Vollblau)",
                SpeedSteps = 128,
                Address = 49,
                TopSpeed = 200,
                Functions =
                {
                    new DccFunction(0, "Headlights", -1),
                    new DccFunction(1, "Sound", 0),
                    new DccFunction(2, "Horn high - long", 0),
                    new DccFunction(3, "Horn low - long", 0),
                    new DccFunction(4, "Compressor", 0),
                    new DccFunction(5, "Couple/decouple Sound", 1000),
                    new DccFunction(6, "Shunting Gear + Shungting Light", 0),
                    new DccFunction(7, "High Beams", 0),
                    new DccFunction(8, "Horn high - short", 500),
                    new DccFunction(9, "Horn low - short", 500),
                    new DccFunction(10, "Cab lighting (at standstill)", 0),
                    new DccFunction(11, "Curve squeaking", 0),
                    new DccFunction(12, "Conductor Whistle", 0),
                    new DccFunction(13, "Passing train", 0),
                    new DccFunction(14, "Mute", 0),
                    new DccFunction(15, "Headlights + Single White", 0),
                    new DccFunction(16, "Headlights + Double Reds", 0),
                    new DccFunction(17, "Headlights + Single Red", 0),
                    new DccFunction(18, "Double Reds", 0),
                    new DccFunction(19, "Single Red", 0),
                    new DccFunction(20, "None (Light)", 0),
                    new DccFunction(21, "Sequence \"Zwangsbremsung\"", 0),
                    new DccFunction(22, "Doors", 0),
                    new DccFunction(23, "Störung Störung Störung", 0),
                    new DccFunction(24, "Zugbeeinflussung 3x", 0),
                    new DccFunction(25, "SIFA", 0),
                    new DccFunction(26, "Brake squeal", 0),
                    new DccFunction(27, "Decrease volume", 0),
                    new DccFunction(28, "Inrcease volume", 0),
                },
            },

            new Engine()
            {
                Name = "Roco BR 193 492 (Hupac Nightpiercer)",
                SpeedSteps = 128,
                Address = 49,
                TopSpeed = 200,
                Functions =
                {
                    new DccFunction(0, "Headlights", 0),
                    new DccFunction(1, "Sound", 0),
                    new DccFunction(2, "Horn high - long", 0),
                    new DccFunction(3, "Horn low - long", 0),
                    new DccFunction(4, "Compressor", 0),
                    new DccFunction(5, "Couple/decouple Sound", 1000),
                    new DccFunction(6, "Shunting Gear + Shungting Light", 0),
                    new DccFunction(7, "High Beams", 0),
                    new DccFunction(8, "Horn high - short", 500),
                    new DccFunction(9, "Horn low - short", 500),
                    new DccFunction(10, "Cab lighting (at standstill)", 0),
                    new DccFunction(11, "Curve squeaking", 0),
                    new DccFunction(12, "Conductor Whistle", 0),
                    new DccFunction(13, "Passing train", 0),
                    new DccFunction(14, "Mute", 0),
                    new DccFunction(15, "Headlights + Single White", 0),
                    new DccFunction(16, "Headlights + Double Reds", 0),
                    new DccFunction(17, "Headlights + Single Red", 0),
                    new DccFunction(18, "Double Reds", 0),
                    new DccFunction(19, "Single Red", 0),
                    new DccFunction(20, "None (Light)", 0),
                    new DccFunction(21, "Sequence \"Zwangsbremsung\"", 0),
                    new DccFunction(22, "Doors", 0),
                    new DccFunction(23, "Störung Störung Störung", 0),
                    new DccFunction(24, "Zugbeeinflussung 3x", 0),
                    new DccFunction(25, "SIFA", 0),
                    new DccFunction(26, "Brake squeal", 0),
                    new DccFunction(27, "Decrease volume", 0),
                    new DccFunction(28, "Inrcease volume", 0),
                },
            },

            new Engine()
            {
                Name = "Roco BR 193 Gotthardo (SBB C Int)",
                SpeedSteps = 128,
                TopSpeed = 200,
                Functions =
                {
                    new DccFunction(0, "Headlights", 0),
                    new DccFunction(1, "Sound", 0),
                    new DccFunction(2, "Horn high - long", 0),
                    new DccFunction(3, "Horn low - long", 0),
                    new DccFunction(4, "Compressor", 0),
                    new DccFunction(5, "Couple/decouple Sound", 1000),
                    new DccFunction(6, "Shunting Gear + Shungting Light", 0),
                    new DccFunction(7, "High Beams", 0),
                    new DccFunction(8, "Horn high - short", 500),
                    new DccFunction(9, "Horn low - short", 500),
                    new DccFunction(10, "Cab lighting (at standstill)", 0),
                    new DccFunction(11, "Curve squeaking", 0),
                    new DccFunction(12, "Conductor Whistle", 0),
                    new DccFunction(13, "Passing train", 0),
                    new DccFunction(14, "Mute", 0),
                    new DccFunction(15, "Headlights + Single White", 0),
                    new DccFunction(16, "Headlights + Double Reds", 0),
                    new DccFunction(17, "Headlights + Single Red", 0),
                    new DccFunction(18, "Double Reds", 0),
                    new DccFunction(19, "Single Red", 0),
                    new DccFunction(20, "None (Light)", 0),
                    new DccFunction(21, "Sequence \"Zwangsbremsung\"", 0),
                    new DccFunction(22, "Doors", 0),
                    new DccFunction(23, "Störung Störung Störung", 0),
                    new DccFunction(24, "Zugbeeinflussung 3x", 0),
                    new DccFunction(25, "SIFA", 0),
                    new DccFunction(26, "Brake squeal", 0),
                    new DccFunction(27, "Decrease volume", 0),
                    new DccFunction(28, "Inrcease volume", 0),
                },
            },

            new Engine()
            {
                Name = "Märklin Re 474 003",
                SpeedSteps = 128,
                TopSpeed = 140,
                Functions =
                {
                    new DccFunction(0, "Headlights / Red marker light", 0),
                    new DccFunction(1, "HIgh beams", 0),
                    new DccFunction(2, "Operating Sounds", 0),
                    new DccFunction(3, "High pitched horn", 0),
                    new DccFunction(4, "ABV (off)", 0),
                    new DccFunction(5, "Brake squealing (off)", 0),
                    new DccFunction(6, "Headlights Engineer‘s Cab 2 off", 1),
                    new DccFunction(7, "Low pitched horn", 0),
                    new DccFunction(8, "Headlights Engineer‘s Cab 1 off", 0),
                    new DccFunction(9, "Compressor", 0),
                    new DccFunction(10, "Compressed air", 0),
                    new DccFunction(11, "Station announcement", 2000),
                    new DccFunction(12, "Conductor whistle", 0),
                    new DccFunction(13, "Sanding", 0),
                    new DccFunction(14, "Coupling / uncoupling", 1000),
                    new DccFunction(15, "Squealing brakes", 1000),
                },
            },
            new Engine()
            {
                Name = "Märklin Re 460 Circus Knie",
                SpeedSteps = 128,
                TopSpeed = 200,
                Functions =
                {
                    new DccFunction(0, "Headlights with „Swiss headlight changeover“", 0),
                    new DccFunction(1, "Switching marker lights(1 x white -> 1 x red)", 0),
                    new DccFunction(2, "Operating sounds", 0),
                    new DccFunction(3, "Sound effect: Horn", 500),
                    new DccFunction(4, "Long distance headlights", 0),
                    new DccFunction(5, "Engineer‘s cab lighting", 0),
                    new DccFunction(6, "Headlights Engineer‘s Cab 2 off", 0),
                    new DccFunction(7, "Sound effect: Short horn blast", 500),
                    new DccFunction(8, "Headlights Engineer‘s Cab 1 off", 0),
                    new DccFunction(9, "Sound effect: Squealing brakes off", 0),
                    new DccFunction(10, "ABV, off", 0),
                    new DccFunction(11, "Sound effect: Blower", 0),
                    new DccFunction(12, "Sound effect: Conductor whistle", 0),
                    new DccFunction(13, "Sound effect: Compressor", 0),
                    new DccFunction(14, "Sound effect: Letting off air", 0),
                    new DccFunction(15, "Sound effect: Squealing brakes on", 0),
                    new DccFunction(16, "Marker lights (2 x red)", 0),
                    new DccFunction(17, "Sound effect: Sanding", 0),
                    new DccFunction(18, "Low speed switching range + switching lights", 0),
                    new DccFunction(19, "Train announcement 1", 1000),
                    new DccFunction(20, "Warning signal (red)", 0),
                    new DccFunction(21, "Sound effect: Doors being closed", 500),
                    new DccFunction(22, "Wrong track running in Switzerland(1 x red, 2 x white)", 0),
                    new DccFunction(23, "Station announcements 1", 4000),
                    new DccFunction(24, "Station announcements 2", 4000),
                    new DccFunction(25, "Station announcements 3", 2000),
                    new DccFunction(26, "Station announcements 1", 3000),
                    new DccFunction(27, "Train announcements 2", 2000),
                    new DccFunction(28, "Train announcements 3", 4000),
                },
            },
            new Engine
            {
                Name = "Märklin Re 620 X-Rail",
                SpeedSteps = 128,
                TopSpeed = 140,
            },

            new Engine
            {
                Name = "Märklin Re 620 058",
                SpeedSteps = 128,
                TopSpeed = 140,
            },
            new Engine
            {
                Name = "Märklin Re 6/6 Balerna",
                SpeedSteps = 128,
                TopSpeed = 140,
            },
            new Engine
            {
                Name = "Märklin Re 420 Circus Knie",
                SpeedSteps = 128,
                TopSpeed = 140,
            },
            new Engine
            {
                Name = "Märklin Re 482 036 (SBB Cargo)",
                SpeedSteps = 128,
                TopSpeed = 140,
            },
            new Engine
            {
                Name = "ES 64 F4 - 063 (MRCE)",
                SpeedSteps = 128,
                TopSpeed = 140,
            },

            new Engine
            {
                Name = "Black Pearl",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "Cat's Eye",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "Re 475 BLS Alpinist",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "BR 193 \"Das ist Grün\"",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "BR 193 (Railpool)",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "BR 193 (MRCE)",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "BR 193 (DB Cargo)",
                SpeedSteps = 128,
                TopSpeed = 200,
            },

            new Engine
            {
                Name = "BLS Re 4/4 185",
                SpeedSteps = 128,
                TopSpeed = 140,
            },

            new Engine
            {
                Name = "BLS Re 4/4 174",
                SpeedSteps = 128,
                TopSpeed = 140,
            },

            new Engine
            {
                Name = "BLS Re 4/4 194",
                SpeedSteps = 128,
                TopSpeed = 140,
            },

            new Engine
            {
                Name = "SBB Re 460 \"Munot\"",
                SpeedSteps = 128,
                TopSpeed = 200,
            },
        };
    }
}