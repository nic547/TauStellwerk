// <copyright file="StatusService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Services;

public class StatusService
{
    private readonly HttpClientService _service;

    public StatusService(HttpClientService httpClientService)
    {
        _service = httpClientService;
        _ = Init();
    }

    public delegate void StatusChangeHandler(Status status);

    public event StatusChangeHandler? StatusChanged;

    public Status? LastKnownStatus { get; private set; }

    public async Task SetStatus(Status status)
    {
        var client = await _service.GetHubConnection();
        await client.SendAsync("SetStatus", status);
        StatusChanged?.Invoke(status);
        LastKnownStatus = status;

        // var json = JsonSerializer.Serialize(status, TauJsonContext.Default.Status);
        // StatusChanged?.Invoke(status);
        // LastKnownStatus = status;
        // _ = await client.PostAsync("/status", new StringContent(json, Encoding.UTF8, "text/json"));
    }

    private async Task Init()
    {
        var connection = await _service.GetHubConnection();
        connection.On<Status>("HandleStatusChange", HandleStatusChange);
        var currentStatus = await connection.InvokeAsync<Status>("GetStatus");
        HandleStatusChange(currentStatus);
    }

    private void HandleStatusChange(Status newStatus)
    {
        LastKnownStatus = newStatus;
        StatusChanged?.Invoke(newStatus);
    }
}