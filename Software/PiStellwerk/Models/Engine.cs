using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiStellwerk.Models
{
    public class Engine
    {
        public string Name { get; set; }
        public int Address { get; set; }
        public byte SpeedSteps { get; set; }
        public int TopSpeed { get; set; }

        public SpeedDisplayType SpeedDisplayType { get; set; }

        public List<DccFunction> Functions { get; set; }
    }

    public class DccFunction
    {
        public DccFunction() { }
        public DccFunction(byte id, string name)
        {
            Id = id;
            Name = name;
        }
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    public enum FunctionType : byte
    {
        Sound = 0,
        Light = 1,
        Physical = 2
    }

    public enum FunctionKind : byte
    {
        Momentary = 0,
        Continus = 1,

    }

    public enum SpeedDisplayType : byte
    {
        /// <summary>
        /// Speed is supposed to be displayed as percent, from 0% to 100%
        /// </summary>
        Percent,
        /// <summary>
        /// Speed is supposed to be displayed as the actual dcc speed steps
        /// </summary>
        SpeedSteps,
        /// <summary>
        /// Speed is supposed to be displayed as the appoximation of speed based on the top speed and speed steps.
        /// </summary>
        TopSpeed,
    }
}
