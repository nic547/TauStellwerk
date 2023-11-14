// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR.Client;

namespace TauStellwerk.Client.Services.Connections;

public interface IConnectionService
{
    public event EventHandler<HubConnection?>? ConnectionChanged;

    Task<HubConnection?> TryGetHubConnection();

    Task<HttpClient?> TryGetHttpClient();
}
