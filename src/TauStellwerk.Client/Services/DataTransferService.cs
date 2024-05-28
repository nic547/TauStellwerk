// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Client.Services.Connections;

namespace TauStellwerk.Client.Services;
public class DataTransferService
{
    private readonly IConnectionService _connectionService;

    public event EventHandler<string>? BackupCreated;

    public DataTransferService(IConnectionService connectionService)
    {
        _connectionService = connectionService;

        ConnectionSetUp();
    }

    private async void ConnectionSetUp()
    {
        var connection = await _connectionService.TryGetHubConnection();
        connection?.On("BackupCreated", (string newBackup) => { BackupCreated?.Invoke(null, newBackup); });
    }

    public async Task<List<string>> GetBackups()
    {
        var hubConnection = await _connectionService.TryGetHubConnection();
        if (hubConnection == null)
        {
            return [];
        }

        return await hubConnection.InvokeAsync<List<string>>("GetBackups");
    }

    public async Task StartBackup()
    {
        var hubConnection = await _connectionService.TryGetHubConnection();
        if (hubConnection == null)
        {
            return;
        }

        await hubConnection.InvokeAsync("StartBackup");
    }
}
