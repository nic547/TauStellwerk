// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base.Dto;
using TauStellwerk.Client.Services.Connections;

namespace TauStellwerk.Client.Services.DecoderProgramming;
public class DecoderProgrammingService : IDecoderProgrammingService
{
    private readonly IConnectionService _connectionService;

    public DecoderProgrammingService(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<ResultDto<int>> ReadDccAddress()
    {
        var connection = await _connectionService.TryGetHubConnection();
        if (connection == null)
        {
            return new ResultDto<int>(default, false, "Failed to connect to Server");
        }

        return await connection.InvokeAsync<ResultDto<int>>("ReadDccAddress");

    }

    public async Task<ResultDto> WriteDccAddress(int address)
    {
        var connection = await _connectionService.TryGetHubConnection();
        if (connection == null)
        {
            return new ResultDto(false, "Failed to connect to Server");
        }

        return await connection.InvokeAsync<ResultDto>("WriteDccAddress", address);
    }
}
