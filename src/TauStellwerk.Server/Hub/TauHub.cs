// <copyright file="TauHub.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using TauStellwerk.Server.Dao;
using TauStellwerk.Server.Services;
using TauStellwerk.Server.Services.EngineService;

namespace TauStellwerk.Server.Hub;

public partial class TauHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly SessionService _sessionService;
    private readonly StatusService _statusService;
    private readonly IEngineService _engineService;
    private readonly TurnoutService _turnoutService;
    private readonly EngineDao _engineDao;
    private readonly ITurnoutDao _turnoutDao;
    private readonly ILogger<TauHub> _logger;

    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
    public TauHub(
        SessionService sessionService,
        StatusService statusService,
        IEngineService engineService,
        TurnoutService turnoutService,
        EngineDao engineDao,
        ITurnoutDao turnoutDao,
        ILogger<TauHub> logger)
    {
        _sessionService = sessionService;
        _statusService = statusService;
        _engineService = engineService;
        _turnoutService = turnoutService;
        _engineDao = engineDao;
        _turnoutDao = turnoutDao;
        _logger = logger;
    }
}
