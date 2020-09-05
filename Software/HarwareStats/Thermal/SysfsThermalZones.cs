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
            var i = 0;
            var stats = new List<InfoRecord>();

            while (true)
            {
                string name;
                string temp;
                try
                {
                    temp = File.ReadLines($"/sys/class/thermal/thermal_zone{i}/temp").First();
                    name = File.ReadLines($"/sys/class/thermal/thermal_zone{i}/type").First();
                }
                catch (IOException)
                {
                    if (i == 0)
                    {
                        throw new NotSupportedException();
                    }

                    break;
                }

                stats.Add(new InfoRecord
                {
                    Type = InfoType.Thermal,
                    Name = name,
                    Value = $"Temperature: {Math.Round(double.Parse(temp) / 1000d, 1)}°C",
                });
                i++;
            }

            return stats;
        }
    }
}
