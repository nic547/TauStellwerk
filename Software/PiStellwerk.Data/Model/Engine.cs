using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PiStellwerk.Data
{
    public class Engine
    {
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The DCC Address of an Engine. Is not necessarily unique.
        /// </summary>
        public ushort Address { get; set; }
        public byte SpeedSteps { get; set; }
        public int TopSpeed { get; set; }

        public SpeedDisplayType SpeedDisplayType { get; set; }

        public List<DccFunction> Functions { get; set; }

        /// <summary>
        /// Gets or sets a list of strings that describe an engine. These might be alternative names, manufacturers, the owner etc, basically
        /// everything one might search for if the exact name is unkown. 
        /// </summary>
        public List<string> Tags { get; set; }
    }

    public class DccFunction
    {
        public DccFunction() { }
        public DccFunction(byte number, string name)
        {
            Number = number;
            Name = name;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte Number { get; set; }
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
