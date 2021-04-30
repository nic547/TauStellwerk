// <copyright file="SpeedDisplayType.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data
{
    /// <summary>
    /// Represents different ways speed of displaying the speed of a model train.
    /// </summary>
    public enum SpeedDisplayType : byte
    {
        /// <summary>
        /// Speed is supposed to be displayed as percent, from 0% to 100%.
        /// </summary>
        Percent,

        /// <summary>
        /// Speed is supposed to be displayed as the actual dcc speed steps.
        /// </summary>
        SpeedSteps,

        /// <summary>
        /// Speed is supposed to be displayed as the approximation of speed based on the top speed and speed steps.
        /// </summary>
        TopSpeed,
    }
}