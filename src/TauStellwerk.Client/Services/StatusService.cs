// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base;

namespace TauStellwerk.Client.Services;

public class StatusService
{
    private readonly IConnectionService _service;

    public StatusService(IConnectionService connectionService)
    {
        _service = connectionService;
        _service.ConnectionChanged += async (_, _) => await Init();
        _ = Init();
    }

    public event EventHandler<SystemStatus?>? StatusChanged;

    public SystemStatus? LastKnownStatus { get; private set; }

    public async Task SetStatus(SystemStatus systemStatus)
    {
        var client = await _service.TryGetHubConnection();
        if (client is null)
        {
            return;
        }

        await client.SendAsync("SetStatus", systemStatus);
        StatusChanged?.Invoke(this, systemStatus);
        LastKnownStatus = systemStatus;
    }

    private async Task Init()
    {
        HandleStatusChange(null);
        var connection = await _service.TryGetHubConnection();
        if (connection is null)
        {
            return;
        }

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