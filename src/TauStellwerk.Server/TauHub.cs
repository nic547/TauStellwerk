// <copyright file="TauHub.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR;
using TauStellwerk.Services;

namespace TauStellwerk;

public partial class TauHub : Hub
{
    private readonly SessionService _sessionService;

    public TauHub(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public void RegisterUser(string username)
    {
        _sessionService.CreateSession(username, null, Context.ConnectionId);
    }

    public void SendHeartbeat()
    {
        _sessionService.TryUpdateSessionLastContact(Context.ConnectionId);
    }
}
