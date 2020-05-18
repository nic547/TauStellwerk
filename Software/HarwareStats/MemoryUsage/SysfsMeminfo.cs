using HardwareInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HardwareStats.MemoryUsage
{
    class SysfsMeminfo : IStatProvider
    {
        public bool CheckAvailability()
        {
            throw new NotImplementedException();
        }

        public IList<Stat> GetStats()
        {
            using (var stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    var totalMemory = long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                    _ = reader.ReadLine();
                    line = reader.ReadLine();
                    var availableMemory = long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                    double memoryPercent = (totalMemory - availableMemory) * 100 / totalMemory;
                    Console.WriteLine($"Memory Usage: {Math.Round(memoryPercent)}% total:{totalMemory / 1000}MB available:{availableMemory / 1000}MB");
                }
            }
        }
    }
}
