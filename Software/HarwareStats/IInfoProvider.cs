// <copyright file="IInfoProvider.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace HardwareInfo
{
    /// <summary>
    /// Designates classês that provides Information about state of the hardware.
    /// </summary>
    public interface IInfoProvider
    {
        /// <summary>
        /// Check if a IInfoProvider works on the current system.
        /// </summary>
        /// <returns><see cref="bool"/> indicating the availability of a IInfoProvider.</returns>
        bool CheckAvailability();

        /// <summary>
        /// Get the Information a IInfoProvider provides.
        /// </summary>
        /// <returns>A list of <see cref="InfoRecord"/>.</returns>
        IList<InfoRecord> GetInfoRecords();
    }
}
