// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    public SystemStatus GetStatus()
    {
        return _statusControlService.CheckStatus();
    }

    public async Task SetStatus(SystemStatus systemStatus)
    {
        _logger.LogDebug("StatusChange received: {status}", systemStatus.State);
        await _statusControlService.HandleStatusCommand(systemStatus.State, systemStatus.LastActionUsername);
        await Clients.Others.SendAsync("HandleStatusChange", systemStatus);
    }
}
