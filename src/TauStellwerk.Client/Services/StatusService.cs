// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base;

namespace TauStellwerk.Client.Services;

public class StatusService
{
    private readonly ConnectionService _service;

    public StatusService(ConnectionService connectionService)
    {
        _service = connectionService;
        _service.ConnectionChanged += async (_, _) => await Init();
        _ = Init();
    }

    public event EventHandler<SystemStatus?>? StatusChanged;

    public SystemStatus? LastKnownStatus { get; private set; }

    public async Task SetStatus(SystemStatus systemStatus)
    {
        var client = await _service.GetHubConnection();
        await client.SendAsync("SetStatus", systemStatus);
        StatusChanged?.Invoke(this, systemStatus);
        LastKnownStatus = systemStatus;
    }

    private async Task Init()
    {
        HandleStatusChange(null);
        var connection = await _service.GetHubConnection();
        connection.On<SystemStatus>("HandleStatusChange", HandleStatusChange);
        var currentStatus = await connection.InvokeAsync<SystemStatus>("GetStatus");
        HandleStatusChange(currentStatus);
    }

    private void HandleStatusChange(SystemStatus? newSystemStatus)
    {
        LastKnownStatus = newSystemStatus;
        StatusChanged?.Invoke(this, newSystemStatus);
    }
}