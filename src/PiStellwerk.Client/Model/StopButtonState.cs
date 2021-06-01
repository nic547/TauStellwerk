// <copyright file="StopButtonState.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Timers;
using JetBrains.Annotations;
using PiStellwerk.Data;

namespace PiStellwerk.Client.Model
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

        private State _state = State.Unknown;

        public StopButtonState()
        {
            _lockingTimer.Elapsed += UnlockState;
            _lockingTimer.AutoReset = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private enum State
        {
            Unknown,
            Running,
            StoppedLocked,
            Stopped,
        }

        public bool ShouldBeEnabled => _state != State.StoppedLocked && _state != State.Unknown;

        public bool ShouldBeDisabled => !ShouldBeEnabled;

        public string TitleText
        {
            get
            {
                return _state switch
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
                return _state switch
                {
                    State.Unknown => "PiStellwerk is in unknown state",
                    State.Running => $"PiStellwerk started by {_lastActionUsername}",
                    State.Stopped or State.StoppedLocked => $"PiStellwerk started by {_lastActionUsername}",
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        public void SetStatus(Status status)
        {
            _lockingTimer.Stop();
            _lastActionUsername = status.LastActionUsername;
            if (status.IsRunning)
            {
                _state = State.Running;
            }
            else
            {
                _state = State.StoppedLocked;
                _lockingTimer.Start();
            }
            OnPropertyChanged();
        }

        private void UnlockState(object source, ElapsedEventArgs e)
        {
            if (_state == State.StoppedLocked)
            {
                _state = State.Stopped;
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