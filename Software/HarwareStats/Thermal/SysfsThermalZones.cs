using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace HardwareInfo.Thermal
{
    class SysfsThermalZones : IStatProvider
    {
        public bool CheckAvailability()
        {
            try
            {
                _ = GetStats();
                Console.WriteLine("SysfsThermalZones is available on this device");
                return true;
            }

            catch
            {
                Console.WriteLine("SysfsThermalZones is NOT available on this device");
                return false;
            }
        }

        public IList<Stat> GetStats()
        {
            //TODO: Read all thermal zones. Not done yet b/c my RaspberryPi used for testing has only one. 
            List<Stat> stats = new List<Stat>();
            var temp = File.ReadLines("/sys/class/thermal/thermal_zone0/temp").First();
            var name = File.ReadLines("/sys/class/thermal/thermal_zone0/type").First();

            stats.Add(new Stat()
                {
                    Type = StatType.Thermal,
                    Name = name,
                    Value = $"Temperature: {Math.Round(double.Parse(temp) / 1000d, 1)}°C"
                });            
            return stats;
        }
    }
}
