// <copyright file="TauHubSession.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Hub;

public partial class TauHub
{
    public void RegisterUser(string username)
    {
        _sessionService.CreateSession(username, null, Context.ConnectionId);
    }

    public void SendHeartbeat()
    {
        _sessionService.TryUpdateSessionLastContact(Context.ConnectionId);
    }
}