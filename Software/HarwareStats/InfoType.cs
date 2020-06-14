// <copyright file="InfoType.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HardwareInfo
{
    /// <summary>
    /// Rough indication of what system an InfoRecord is related to.
    /// </summary>
    public enum InfoType
    {
        /// <summary>
        /// Indicates that an InfoRecord is related to temperature.
        /// </summary>
        Thermal,

        /// <summary>
        /// Indicates that an InfoRecord is related to memory consumption.
        /// </summary>
        MemoryUsage,

        /// <summary>
        /// Indicates that an InfoRecord is related to cpu usage.
        /// </summary>
        CpuUsage,
    }
}