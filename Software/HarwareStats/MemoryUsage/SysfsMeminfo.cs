using HardwareInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HardwareStats.MemoryUsage
{
    class SysfsMeminfo : IStatProvider
    {

        private readonly StringSplitOptions _splitOptions = StringSplitOptions.RemoveEmptyEntries;
        private readonly char[] _space = new[] {' '};

        public bool CheckAvailability()
        {
            try
            {
                GetStats();
                Console.WriteLine("SysfsMeminfo is available on this device");
                return true;
            }

            catch (Exception)
            {
                Console.WriteLine("SysfsMeminfo is NOT available on this device");
                return false;
            }
        }

        public IList<Stat> GetStats()
        {
            using (var stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var results = new List<Stat>
                    {
                        new Stat(StatType.MemoryUsage, "MemTotal", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First()),
                        new Stat(StatType.MemoryUsage, "MemFree", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First()),
                        new Stat(StatType.MemoryUsage, "MemAvailable", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First())
                    };
                    return results;
                }
            }
        }
    }
}
