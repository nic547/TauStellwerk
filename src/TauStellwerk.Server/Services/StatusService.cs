// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Hub;

namespace TauStellwerk.Services;

public class StatusService
{
    private readonly CommandSystemBase _system;
    private readonly IHubContext<TauHub> _hubContext;
    private readonly ILogger<StatusService> _logger;

    private State? _lastKnownState;
    private string _lastActionUsername = "SYSTEM";

    public StatusService(CommandSystemBase system, IHubContext<TauHub> hubContext, ILogger<StatusService> logger, SessionService sessionService, IOptions<TauStellwerkOptions> options)
    {
        _system = system;
        _hubContext = hubContext;
        _logger = logger;

        if (options.Value.StopOnLastUserDisconnect)
        {
            sessionService.NoUsersRemaining += async () => await HandleLastUserDisconnected();
        }

        _system.StatusChanged += HandleStatusEvent;
        _system.CheckState();
    }

    public SystemStatus CheckStatus()
    {
        return new() { State = _lastKnownState ?? State.Off, LastActionUsername = _lastActionUsername };
    }

    public async Task HandleStatusCommand(State state, string username)
    {
        var task = _system.HandleSystemStatus(state);

        _lastKnownState = state;
        _lastActionUsername = username;

        await task;
    }

    private async Task HandleLastUserDisconnected()
    {
        await HandleStatusCommand(State.Off, "SYSTEM");
        _logger.LogInformation("Last user disconnected, stopping CommandStation");
    }

    private void HandleStatusEvent(State state)
    {
        if (_lastKnownState == state)
        {
            return;
        }

        _lastKnownState = state;
        _lastActionUsername = "SYSTEM";
        _logger.LogInformation($"SYSTEM {(state == State.On ? "started" : "stopped")} the TauStellwerk");

        SystemStatus systemStatus = new()
        {
            State = state,
            LastActionUsername = _lastActionUsername,
        };
        _hubContext.Clients.All.SendAsync("HandleStatusChange", systemStatus);
    }
}