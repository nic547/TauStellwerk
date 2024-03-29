﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.Timer;

public class FakeTimer : ITimer
{
    public event EventHandler<DateTime>? Elapsed;

    public double Interval { get; set; }

    public bool AutoReset { get; set; }

    public int StartCounter { get; private set; }

    public int StopCounter { get; private set; }

    public void Start()
    {
        StartCounter++;
    }

    public void Stop()
    {
        StopCounter++;
    }

    public void Elapse()
    {
        Elapsed?.Invoke(this, DateTime.UtcNow);
    }

    public void Elapse(DateTime signalTime)
    {
        Elapsed?.Invoke(this, signalTime);
    }
}
