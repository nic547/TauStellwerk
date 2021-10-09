// <copyright file="StopButtonState.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Timers;
using JetBrains.Annotations;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model
{
    public class StopButtonState : INotifyPropertyChanged
    {
        private const int _lockingSeconds = 3;

        private readonly Timer _lockingTimer = new(TimeSpan.FromSeconds(_lockingSeconds).TotalMilliseconds)
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

        public bool ShouldBeEnabled => CurrentState != State.StoppedLocked && CurrentState != State.Unknown;

        public bool ShouldBeDisabled => !ShouldBeEnabled;

        public string TitleText
        {
            get
            {
                return CurrentState switch
                {
                    State.Unknown => "UNKNOWN",
                    State.Running => "RUNNING",
                    State.StoppedLocked => "STOPPED (LOCKED)",
                    State.Stopped => "STOPPED",
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        public string BottomText
        {
            get
            {
                return CurrentState switch
                {
                    State.Unknown => "TauStellwerk is in unknown state",
                    State.Running => $"TauStellwerk started by {_lastActionUsername}",
                    State.Stopped or State.StoppedLocked => $"TauStellwerk started by {_lastActionUsername}",
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        public State CurrentState { get; private set; } = State.Unknown;

        public void SetStatus(Status status)
        {
            _lockingTimer.Stop();
            _lastActionUsername = status.LastActionUsername;
            if (status.IsRunning)
            {
                CurrentState = State.Running;
            }
            else
            {
                CurrentState = State.StoppedLocked;
                _lockingTimer.Start();
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
            Console.WriteLine("StopButton state changed.");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}