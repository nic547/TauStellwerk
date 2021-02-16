// <copyright file="PrivateMemorySize64.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using PiStellwerk.Util;

namespace HardwareInfo.MemoryUsage
{
    /// <summary>
    /// Reads the PrivateMemorySize64 of the current process. Should not fail on any Platform.
    /// </summary>
    [UsedImplicitly]
    public class PrivateMemorySize64 : IInfoProvider
    {
        /// <inheritdoc />
        public bool CheckAvailability()
        {
            try
            {
                _ = GetInfoRecords();
                ConsoleService.PrintMessage("PrivateMemorySize64 is available on this device");
                return true;
            }
            catch (Exception)
            {
                ConsoleService.PrintError("PrivateMemorySize64 is NOT available on this device. This should not happen!");
                return false;
            }
        }

        /// <inheritdoc />
        public IList<InfoRecord> GetInfoRecords()
        {
            return new List<InfoRecord>
            {
                new InfoRecord()
                {
                    Name = "PrivateMemorySize64",
                    Type = InfoType.MemoryUsage,
                    Value = Process.GetCurrentProcess().PrivateMemorySize64.ToString(),
                },
            };
        }
    }
}