using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareInfo
{
    public class Stat
    {
        public StatType Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }


    public enum StatType
    {
        Thermal,
        MemoryUsage,
        CpuUsage
    }
}
