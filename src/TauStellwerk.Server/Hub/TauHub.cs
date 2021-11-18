// <copyright file="TauHub.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Services;

namespace TauStellwerk.Hub;

public partial class TauHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly SessionService _sessionService;
    private readonly StatusService _statusService;

    public TauHub(SessionService sessionService, StatusService statusService)
    {
        _sessionService = sessionService;
        _statusService = statusService;
    }
}
