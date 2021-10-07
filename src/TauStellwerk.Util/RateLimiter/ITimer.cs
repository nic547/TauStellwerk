// <copyright file="ITimer.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Timers;

namespace TauStellwerk.Util
{
    public interface ITimer
    {
        public event ElapsedEventHandler? Elapsed;

        public double Interval { get; set; }

        public bool AutoReset { get; set; }

        public void Start();
    }
}
