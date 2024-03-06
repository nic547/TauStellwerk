// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    public override Task OnConnectedAsync()
    {
        // The username is sent as AccessToken because it seemed like the easiest way to get SignalR to pass it along.
        // The AccessToken is can be sent via Query oder Header, depending on the Client.
        var username = "unnamed";

        var usernameFromQuery = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
        var usernameFromHeader = Context.GetHttpContext()?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(usernameFromQuery))
        {
            username = usernameFromQuery;
        }

        if (!string.IsNullOrEmpty(usernameFromHeader))
        {
            username = usernameFromHeader[7..]; // Strip "Bearer " from the start of the string
        }

        _sessionService.HandleConnected(Context.ConnectionId, username);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _sessionService.HandleDisconnected(Context.ConnectionId, exception);
        return base.OnDisconnectedAsync(exception);
    }
}
