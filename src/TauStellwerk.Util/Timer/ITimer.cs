// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.Timer;

public interface ITimer
{
    public event EventHandler<DateTime>? Elapsed;

    public double Interval { get; set; }

    public bool AutoReset { get; set; }

    public void Start();

    public void Stop();
}
