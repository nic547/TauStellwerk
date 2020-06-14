﻿// <copyright file="SysfsLoadAverage.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;

namespace HardwareInfo.CpuUsage
{
    /// <summary>
    /// Reports the LoadAverage of a system fro /proc/loadavg.
    /// </summary>
    internal class SysfsLoadAverage : IInfoProvider
    {
        /// <inheritdoc/>
        public bool CheckAvailability()
        {
            try
            {
                _ = GetInfoRecords();

                Console.WriteLine("SysfsLoadAverage is available on this device");
                return true;
            }
            catch
            {
                Console.WriteLine("SysfsLoadAverage is NOT available on this device");
                return false;
            }
        }

        /// <inheritdoc/>
        public IList<InfoRecord> GetInfoRecords()
        {
            var result = new List<InfoRecord>();
            string value = File.ReadAllText("/proc/loadavg");

            result.Add(new InfoRecord()
            {
                Type = InfoType.CpuUsage,
                Name = "loadavg",
                Value = value,
            });

            return result;
        }
    }
}
