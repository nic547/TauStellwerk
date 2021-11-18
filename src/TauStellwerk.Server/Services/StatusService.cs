// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Hub;
using TauStellwerk.Util;

namespace TauStellwerk.Services;

public class StatusService
{
    private readonly CommandSystemBase _system;
    private readonly IHubContext<TauHub> _hubContext;

    private bool _isRunning;
    private string _lastActionUsername = "SYSTEM";

    public StatusService(CommandSystemBase system, IHubContext<TauHub> hubContext)
    {
        _system = system;
        _hubContext = hubContext;

        _system.StatusChanged += HandleStatusEvent;
        _system.CheckState();
    }

    public Status CheckStatus()
    {
        return new() { IsRunning = _isRunning, LastActionUsername = _lastActionUsername };
    }

    public async Task HandleStatusCommand(bool shouldBeRunning, string username)
    {
        var task = _system.HandleSystemStatus(shouldBeRunning);

        _isRunning = shouldBeRunning;
        _lastActionUsername = username;

        await task;
    }

    private void HandleStatusEvent(bool isRunning)
    {
        _isRunning = isRunning;
        _lastActionUsername = "SYSTEM";
        ConsoleService.PrintMessage($"SYSTEM {(isRunning ? "started" : "stopped")} the TauStellwerk");

        Status status = new()
        {
            IsRunning = _isRunning,
            LastActionUsername = _lastActionUsername,
        };
        _hubContext.Clients.All.SendAsync("HandleStatusChange", status);
    }
}