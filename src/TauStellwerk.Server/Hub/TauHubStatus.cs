// <copyright file="TauHubStatus.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR;
using TauStellwerk.Base;

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
