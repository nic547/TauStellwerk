﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Timers;
using JetBrains.Annotations;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model;

public class StopButtonState : INotifyPropertyChanged
{
    private const int LockingSeconds = 3;

    private readonly System.Timers.Timer _lockingTimer = new(TimeSpan.FromSeconds(LockingSeconds).TotalMilliseconds)
    {
        AutoReset = false,
        Enabled = false,
    };

    private string _lastActionUsername = "Nobody";

    public StopButtonState()
    {
        _lockingTimer.Elapsed += UnlockState;
        _lockingTimer.AutoReset = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public enum State
    {
        Unknown,
        Running,
        StoppedLocked,
        Stopped,
    }

    public bool ShouldBeEnabled => CurrentState is not State.StoppedLocked and not State.Unknown;

    public bool ShouldBeDisabled => !ShouldBeEnabled;

    public string TitleText => CurrentState switch
    {
        State.Unknown => "UNKNOWN",
        State.Running => "RUNNING",
        State.StoppedLocked => "STOPPED (LOCKED)",
        State.Stopped => "STOPPED",
        _ => throw new ArgumentOutOfRangeException(),
    };

    public string BottomText => CurrentState switch
    {
        State.Unknown => "TauStellwerk is in unknown state",
        State.Running => $"TauStellwerk started by {_lastActionUsername}",
        State.Stopped or State.StoppedLocked => $"TauStellwerk started by {_lastActionUsername}",
        _ => throw new ArgumentOutOfRangeException(),
    };

    public State CurrentState { get; private set; } = State.Unknown;

    public void SetStatus(SystemStatus? systemStatus)
    {
        _lockingTimer.Stop();
        _lastActionUsername = systemStatus?.LastActionUsername ?? "UNKOWN";

        if (systemStatus is null)
        {
            CurrentState = State.Unknown;
        }
        else
        {
            if (systemStatus.State == Base.Model.State.On)
            {
                CurrentState = State.Running;
            }
            else
            {
                CurrentState = State.StoppedLocked;
                _lockingTimer.Start();
            }
        }

        OnPropertyChanged();
    }

    private void UnlockState(object? source, ElapsedEventArgs e)
    {
        if (CurrentState == State.StoppedLocked)
        {
            CurrentState = State.Stopped;
        }

        OnPropertyChanged();
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}
