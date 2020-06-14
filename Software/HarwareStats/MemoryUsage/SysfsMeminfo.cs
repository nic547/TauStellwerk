// <copyright file="SysfsMeminfo.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HardwareInfo.MemoryUsage
{
    /// <summary>
    /// IInfoProvider that reports Memory Usage based on /proc/meminfo.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class SysfsMeminfo : IInfoProvider
    {
        private const StringSplitOptions _splitOptions = StringSplitOptions.RemoveEmptyEntries;
        private readonly char[] _space = { ' ' };

        /// <inheritdoc />
        public bool CheckAvailability()
        {
            try
            {
                GetInfoRecords();
                Console.WriteLine("SysfsMeminfo is available on this device");
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("SysfsMeminfo is NOT available on this device");
                return false;
            }
        }

        /// <inheritdoc />
        public IList<InfoRecord> GetInfoRecords()
        {
            using (var stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var results = new List<InfoRecord>
                    {
                        new InfoRecord(InfoType.MemoryUsage, "MemTotal", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First()),
                        new InfoRecord(InfoType.MemoryUsage, "MemFree", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First()),
                        new InfoRecord(InfoType.MemoryUsage, "MemAvailable", reader.ReadLine()?.Split(_space, _splitOptions).Skip(1).First()),
                    };
                    return results;
                }
            }
        }
    }
}
