// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using TauStellwerk.Base;
using TauStellwerk.Server.CommandStations;
using TauStellwerk.Server.Hub;

namespace TauStellwerk.Server.Services;

public class StatusService
{
    private readonly CommandStationBase _station;
    private readonly IHubContext<TauHub> _hubContext;
    private readonly ILogger<StatusService> _logger;

    private State? _lastKnownState;
    private string _lastActionUsername = "SYSTEM";

    public StatusService(CommandStationBase station, IHubContext<TauHub> hubContext, ILogger<StatusService> logger, SessionService sessionService, IOptions<TauStellwerkOptions> options)
    {
        _station = station;
        _hubContext = hubContext;
        _logger = logger;

        if (options.Value.StopOnLastUserDisconnect)
        {
            sessionService.NoUsersRemaining += async () => await HandleLastUserDisconnected();
        }

        _station.StatusChanged += HandleStatusEvent;
        _station.CheckState();
    }

    public SystemStatus CheckStatus()
    {
        return new() { State = _lastKnownState ?? State.Off, LastActionUsername = _lastActionUsername };
    }

    public async Task HandleStatusCommand(State state, string username)
    {
        var task = _station.HandleSystemStatus(state);

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
        _logger.LogInformation("SYSTEM {action} the TauStellwerk", state == State.On ? "started" : "stopped");

        SystemStatus systemStatus = new()
        {
            State = state,
            LastActionUsername = _lastActionUsername,
        };
        _hubContext.Clients.All.SendAsync("HandleStatusChange", systemStatus);
    }
}