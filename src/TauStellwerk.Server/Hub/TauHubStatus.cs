// <copyright file="TauHubStatus.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    public Status GetStatus()
    {
        return _statusService.CheckStatus();
    }

    public async Task SetStatus(Status status)
    {
        await _statusService.HandleStatusCommand(status.IsRunning, status.LastActionUsername);
        await Clients.Others.SendAsync("HandleStatusChange", status);
    }
}
