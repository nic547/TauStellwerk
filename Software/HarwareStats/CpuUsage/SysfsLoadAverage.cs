using HardwareInfo;
using System;
using System.Collections.Generic;
using System.IO;

namespace HardwareStats.CpuUsage
{
    class SysfsLoadAverage : IStatProvider
    {
        public bool CheckAvailability()
        {
            try
            {
                _ = GetStats();

                Console.WriteLine("SysfsLoadAverage is available on this device");
                return true;
            }
            catch
            {
                Console.WriteLine("SysfsLoadAverage is NOT available on this device");
                return false;
            }
        }

        public IList<Stat> GetStats()
        {
            var result = new List<Stat>();
            string value = File.ReadAllText("/proc/loadavg");

            result.Add(new Stat()
            {
                Type = StatType.CpuUsage,
                Name = "loadavg",
                Value = value,
            });

            return result;
        }
    }
}
