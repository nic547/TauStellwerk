// <copyright file="SysfsThermalZones.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HardwareInfo.Thermal
{
    /// <summary>
    /// Provides Information about device temperatures based on /sys/class/thermal.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    internal class SysfsThermalZones : IInfoProvider
    {
        /// <inheritdoc/>
        public bool CheckAvailability()
        {
            try
            {
                _ = GetInfoRecords();
                Console.WriteLine("SysfsThermalZones is available on this device");
                return true;
            }
            catch
            {
                Console.WriteLine("SysfsThermalZones is NOT available on this device");
                return false;
            }
        }

        /// <inheritdoc/>
        public IList<InfoRecord> GetInfoRecords()
        {
            // TODO: Read all thermal zones. Not done yet b/c my RaspberryPi used for testing has only one.
            var stats = new List<InfoRecord>();
            var temp = File.ReadLines("/sys/class/thermal/thermal_zone0/temp").First();
            var name = File.ReadLines("/sys/class/thermal/thermal_zone0/type").First();

            stats.Add(new InfoRecord
            {
                Type = InfoType.Thermal,
                Name = name,
                Value = $"Temperature: {Math.Round(double.Parse(temp) / 1000d, 1)}°C",
            });
            return stats;
        }
    }
}
