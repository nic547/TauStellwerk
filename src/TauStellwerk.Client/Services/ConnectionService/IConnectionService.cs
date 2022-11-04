// <copyright file="IConnectionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.SignalR.Client;

namespace TauStellwerk.Client.Services;

public interface IConnectionService
{
    public event EventHandler<HubConnection?>? ConnectionChanged;

    Task<HubConnection?> TryGetHubConnection();
}