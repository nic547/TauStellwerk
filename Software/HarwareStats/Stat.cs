namespace HardwareInfo
{
    public class Stat
    {
        public StatType Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Stat()
        { }

        public Stat(StatType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }


    public enum StatType
    {
        Thermal,
        MemoryUsage,
        CpuUsage
    }
}
