// <copyright file="TimerWrapper.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Timers;

namespace TauStellwerk.Util;

/// <summary>
/// Wraps <see cref="Timer"/> and implements <see cref="ITimer"/>.
/// </summary>
public class TimerWrapper : ITimer
{
    private readonly Timer _timer;

    public TimerWrapper()
    {
        _timer = new Timer();
        _timer.Elapsed += (sender, args) => Elapsed?.Invoke(sender, args);
    }

    public event ElapsedEventHandler? Elapsed;

    public double Interval { get => _timer.Interval; set => _timer.Interval = value; }

    public bool AutoReset { get => _timer.AutoReset; set => _timer.AutoReset = value; }

    public void Start()
    {
        _timer.Start();
    }
}