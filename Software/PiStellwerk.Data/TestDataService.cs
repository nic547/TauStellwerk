using PiStellwerk.Data;
using System;
using System.Collections.Generic;
namespace PiStellwerk
{
    public static class TestDataService
    {
        public static IReadOnlyList<Engine> GetEngines()
        {
            return new List<Engine>()
            {
                new Engine()
                {
                    Name = "Roco BR 193 493 (Hupac Vollblau)",
                    SpeedSteps = 128,
                    Address = 49,
                    TopSpeed = 200,
                    SpeedDisplayType = SpeedDisplayType.TopSpeed,
                    Functions = new List<DccFunction>()
                    {
                        new DccFunction(0, "Headlights"),
                        new DccFunction(1, "Sound"),
                        new DccFunction(2, "Horn high - long"),
                        new DccFunction(3, "Horn low - long"),
                        new DccFunction(4, "Compressor"),
                        new DccFunction(5, "Couple/decouple Sound"),
                        new DccFunction(6, "Shunting Gear + Shungting Light"),
                        new DccFunction(7, "High Beams"),
                        new DccFunction(8, "Horn high - short"),
                        new DccFunction(9, "Horn low - short"),
                        new DccFunction(10, "Cab lighting (at standstill)"),
                        new DccFunction(11, "Curve squeaking"),
                        new DccFunction(12, "Conductor Whistle"),
                        new DccFunction(13, "Passing train"),
                        new DccFunction(14, "Mute"),
                        new DccFunction(15, "Headlights + Single White"),
                        new DccFunction(16, "Headlights + Double Reds"),
                        new DccFunction(17, "Headlights + Single Red"),
                        new DccFunction(18, "Double Reds"),
                        new DccFunction(19, "Single Red"),
                        new DccFunction(20, "None (Light)"),
                        new DccFunction(21, "Sequence \"Zwangsbremsung\""),
                        new DccFunction(22, "Doors"),
                        new DccFunction(23, "Störung Störung Störung"),
                        new DccFunction(24, "Zugbeeinflussung 3x"),
                        new DccFunction(25, "SIFA"),
                        new DccFunction(26, "Brake squeal"),
                        new DccFunction(27, "Decrease volume"),
                        new DccFunction(28, "Inrcease volume"),
                    },
                    Tags = new List<Tag>()
                    {
                        "Vectron",
                        "Siemens",
                        "Dominic",
                    }
                },

                 new Engine()
                {
                    Name = "Roco BR 193 492 (Hupac Nightpiercer)",
                    SpeedSteps = 128,
                    Address = 49,
                    TopSpeed = 200,
                    SpeedDisplayType = SpeedDisplayType.TopSpeed,
                    Functions = new List<DccFunction>()
                    {
                        new DccFunction(0, "Headlights"),
                        new DccFunction(1, "Sound"),
                        new DccFunction(2, "Horn high - long"),
                        new DccFunction(3, "Horn low - long"),
                        new DccFunction(4, "Compressor"),
                        new DccFunction(5, "Couple/decouple Sound"),
                        new DccFunction(6, "Shunting Gear + Shungting Light"),
                        new DccFunction(7, "High Beams"),
                        new DccFunction(8, "Horn high - short"),
                        new DccFunction(9, "Horn low - short"),
                        new DccFunction(10, "Cab lighting (at standstill)"),
                        new DccFunction(11, "Curve squeaking"),
                        new DccFunction(12, "Conductor Whistle"),
                        new DccFunction(13, "Passing train"),
                        new DccFunction(14, "Mute"),
                        new DccFunction(15, "Headlights + Single White"),
                        new DccFunction(16, "Headlights + Double Reds"),
                        new DccFunction(17, "Headlights + Single Red"),
                        new DccFunction(18, "Double Reds"),
                        new DccFunction(19, "Single Red"),
                        new DccFunction(20, "None (Light)"),
                        new DccFunction(21, "Sequence \"Zwangsbremsung\""),
                        new DccFunction(22, "Doors"),
                        new DccFunction(23, "Störung Störung Störung"),
                        new DccFunction(24, "Zugbeeinflussung 3x"),
                        new DccFunction(25, "SIFA"),
                        new DccFunction(26, "Brake squeal"),
                        new DccFunction(27, "Decrease volume"),
                        new DccFunction(28, "Inrcease volume"),
                    },
                    Tags = new List<Tag>()
                    {
                        new Tag("Vectron"),
                        new Tag("Siemens"),
                        new Tag("Dominic"),
                    }
                 },

                new Engine ()
                {
                    Name = "Roco BR 193 Gotthardo (SBB C Int)",
                    Tags = new List<Tag>()
                    {
                        new Tag("SBB Cargo"),
                        new Tag("Vectron"),
                        new Tag("Siemens"),
                        new Tag("Dominic"),
                        new Tag("Electric"),
                        new Tag("Multisystem")
                    },
                    SpeedSteps = 128,
                    TopSpeed = 200,
                    SpeedDisplayType = SpeedDisplayType.Percent,
                    Functions = new List<DccFunction>()
                    {
                        new DccFunction(0, "Headlights"),
                        new DccFunction(1, "Sound"),
                        new DccFunction(2, "Horn high - long"),
                        new DccFunction(3, "Horn low - long"),
                        new DccFunction(4, "Compressor"),
                        new DccFunction(5, "Couple/decouple Sound"),
                        new DccFunction(6, "Shunting Gear + Shungting Light"),
                        new DccFunction(7, "High Beams"),
                        new DccFunction(8, "Horn high - short"),
                        new DccFunction(9, "Horn low - short"),
                        new DccFunction(10, "Cab lighting (at standstill)"),
                        new DccFunction(11, "Curve squeaking"),
                        new DccFunction(12, "Conductor Whistle"),
                        new DccFunction(13, "Passing train"),
                        new DccFunction(14, "Mute"),
                        new DccFunction(15, "Headlights + Single White"),
                        new DccFunction(16, "Headlights + Double Reds"),
                        new DccFunction(17, "Headlights + Single Red"),
                        new DccFunction(18, "Double Reds"),
                        new DccFunction(19, "Single Red"),
                        new DccFunction(20, "None (Light)"),
                        new DccFunction(21, "Sequence \"Zwangsbremsung\""),
                        new DccFunction(22, "Doors"),
                        new DccFunction(23, "Störung Störung Störung"),
                        new DccFunction(24, "Zugbeeinflussung 3x"),
                        new DccFunction(25, "SIFA"),
                        new DccFunction(26, "Brake squeal"),
                        new DccFunction(27, "Decrease volume"),
                        new DccFunction(28, "Inrcease volume"),
                    }
                },

                new Engine()
                {
                    Name = "Märklin Re 474 003",
                    Tags = new List<Tag>()
                    {
                        "Siemens",
                        "ES 64 F4",
                        "SBB Cargo",
                        "Dominic",
                        "Electric",
                        "Multisystem"
                    },
                    SpeedSteps = 128,
                    TopSpeed = 140,
                    SpeedDisplayType = SpeedDisplayType.TopSpeed,
                    Functions = new List<DccFunction>()
                    {
                        new DccFunction(0, "Headlights / Red marker light"),
                        new DccFunction(1, "HIgh beams"),
                        new DccFunction(2, "Operating Sounds"),
                        new DccFunction(3, "High pitched horn"),
                        new DccFunction(4, "ABV (off)"),
                        new DccFunction(5, "Brake squealing (off)"),
                        new DccFunction(6, "Headlights Engineer‘s Cab 2 off"),
                        new DccFunction(7, "Low pitched horn"),
                        new DccFunction(8, "Headlights Engineer‘s Cab 1 off"),
                        new DccFunction(9, "Compressor"),
                        new DccFunction(10, "Compressed air"),
                        new DccFunction(11, "Station announcement"),
                        new DccFunction(12, "Conductor whistle"),
                        new DccFunction(13, "Sanding"),
                        new DccFunction(14, "Coupling / uncoupling"),
                        new DccFunction(15, "Squealing brakes")
                    }
                }
            };
        }
    }
}
