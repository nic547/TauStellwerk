// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Client.Services.Connections;

namespace TauStellwerk.Client.Services;
public class DataTransferService(IConnectionService connectionService)
{
    private readonly IConnectionService _connectionService = connectionService;

    public async Task<List<string>> GetBackups()
    {
        var hubConnection = await _connectionService.TryGetHubConnection();
        if (hubConnection == null)
        {
            return new List<string>();
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
