// <copyright file="ITimer.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Util;

public interface ITimer
{
    public event EventHandler<DateTime>? Elapsed;

    public double Interval { get; set; }

    public bool AutoReset { get; set; }

    public void Start();

    public void Stop();
}