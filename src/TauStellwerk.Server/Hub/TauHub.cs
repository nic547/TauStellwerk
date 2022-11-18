// <copyright file="TauHub.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Server.Services;
using TauStellwerk.Server.Services.EngineService;

namespace TauStellwerk.Server.Hub;

public partial class TauHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly SessionService _sessionService;
    private readonly StatusService _statusService;
    private readonly IEngineService _engineService;
    private readonly TurnoutService _turnoutService;
    private readonly ILogger<TauHub> _logger;

    public TauHub(
        SessionService sessionService,
        StatusService statusService,
        IEngineService engineService,
        TurnoutService turnoutService,
        ILogger<TauHub> logger)
    {
        _sessionService = sessionService;
        _statusService = statusService;
        _engineService = engineService;
        _turnoutService = turnoutService;
        _logger = logger;
    }
}
