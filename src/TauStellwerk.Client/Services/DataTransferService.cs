// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base.Dto;
using TauStellwerk.Client.Services.Connections;

namespace TauStellwerk.Client.Services;
public class DataTransferService
{
    private readonly IConnectionService _connectionService;

    public event EventHandler<BackupInfoDto>? BackupCreated;
    public event EventHandler<string>? BackupDeleted;

    public DataTransferService(IConnectionService connectionService)
    {
        _connectionService = connectionService;

        ConnectionSetUp();
    }

    private async void ConnectionSetUp()
    {
        var connection = await _connectionService.TryGetHubConnection();
        connection?.On("BackupCreated", (BackupInfoDto newBackup) => { BackupCreated?.Invoke(null, newBackup); });
        connection?.On("BackupDeleted", (string filename) => { BackupDeleted?.Invoke(null, filename); });
    }

    public async Task<List<BackupInfoDto>> GetBackups()
    {
        var hubConnection = await _connectionService.TryGetHubConnection();
        if (hubConnection == null)
        {
            return [];
        }

        return await hubConnection.InvokeAsync<List<BackupInfoDto>>("GetBackups");
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

    public async Task DownloadBackup(string fileName, Uri path)
    {
        var httpClient = await _connectionService.TryGetHttpClient();
        if (httpClient == null)
        {
            return;
        }

        await using var localFile = File.Create(path.AbsolutePath);
        await using var remoteFile = await httpClient.GetStreamAsync($"/backups/{fileName}");

        await remoteFile.CopyToAsync(localFile);
    }

    public async Task DeleteBackup(string filename)
    {
        var connection = await _connectionService.TryGetHubConnection();
        if (connection == null)
        {
            return;
        }

        await connection.InvokeAsync("DeleteBackup", filename);
    }
}
