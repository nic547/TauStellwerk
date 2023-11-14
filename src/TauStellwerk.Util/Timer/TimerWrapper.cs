// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.Timer;

/// <summary>
/// Wraps <see cref="System.Timers.Timer"/> and implements <see cref="ITimer"/>.
/// </summary>
public class TimerWrapper : ITimer
{
    private readonly System.Timers.Timer _timer;

    public TimerWrapper()
    {
        _timer = new();
        _timer.Elapsed += (sender, args) => Elapsed?.Invoke(sender, args.SignalTime);
    }

    public TimerWrapper(int milliseconds)
    {
        _timer = new(milliseconds);
        _timer.Elapsed += (sender, args) => Elapsed?.Invoke(sender, args.SignalTime);
    }

    public event EventHandler<DateTime>? Elapsed;

    public double Interval { get => _timer.Interval; set => _timer.Interval = value; }

    public bool AutoReset { get => _timer.AutoReset; set => _timer.AutoReset = value; }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}
