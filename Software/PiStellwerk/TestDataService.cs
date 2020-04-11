using PiStellwerk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    TopSpeed = 200,
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
                new Engine ()
                {
                    Name = "Roco BR 193 Gotthardo (SBB C Int)",
                    SpeedSteps = 128,
                    TopSpeed = 200,
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
                }
            };
        }
    }
}
