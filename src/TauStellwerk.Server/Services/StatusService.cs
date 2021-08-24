// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Util;

namespace TauStellwerk.Services
{
    public class StatusService
    {
        private readonly CommandSystemBase _system;

        private readonly List<TaskCompletionSource<Status>> _statusAwaiters = new();

        private bool _isRunning;
        private string _lastActionUsername = "SYSTEM";

        public StatusService(CommandSystemBase system)
        {
            _system = system;

            _system.StatusChanged += HandleStatusEvent;
            _system.CheckState();
        }

        public Status CheckStatus()
        {
            return new() { IsRunning = _isRunning, LastActionUsername = _lastActionUsername };
        }

        public async Task<Status> WaitForStatusChangeAsync()
        {
            var tcs = new TaskCompletionSource<Status>();
            lock (_statusAwaiters)
            {
                _statusAwaiters.Add(tcs);
            }

            return await tcs.Task;
        }

        public async Task HandleStatusCommand(bool shouldBeRunning, string username)
        {
            var task = _system.HandleSystemStatus(shouldBeRunning);

            _isRunning = shouldBeRunning;
            _lastActionUsername = username;
            NotifyStatusChanged();

            await task;
        }

        private void HandleStatusEvent(bool isRunning)
        {
            _isRunning = isRunning;
            _lastActionUsername = "SYSTEM";
            ConsoleService.PrintMessage($"SYSTEM {(isRunning ? "started" : "stopped")} the TauStellwerk");
            NotifyStatusChanged();
        }

        private void NotifyStatusChanged()
        {
            Status status = new()
            {
                IsRunning = _isRunning,
                LastActionUsername = _lastActionUsername,
            };

            lock (_statusAwaiters)
            {
                foreach (var awaiter in _statusAwaiters)
                {
                    awaiter.SetResult(status);
                }

                _statusAwaiters.Clear();
            }
        }
    }
}
