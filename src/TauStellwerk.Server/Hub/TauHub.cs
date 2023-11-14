// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Server.Services;
using TauStellwerk.Server.Services.EngineControlService;

namespace TauStellwerk.Server.Hub;

public partial class TauHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly SessionService _sessionService;
    private readonly StatusControlService _statusControlService;
    private readonly IEngineControlService _engineControlService;
    private readonly TurnoutControlService _turnoutControlService;
    private readonly ILogger<TauHub> _logger;

    public TauHub(
        SessionService sessionService,
        StatusControlService statusControlService,
        IEngineControlService engineControlService,
        TurnoutControlService turnoutControlService,
        ILogger<TauHub> logger)
    {
        _sessionService = sessionService;
        _statusControlService = statusControlService;
        _engineControlService = engineControlService;
        _turnoutControlService = turnoutControlService;
        _logger = logger;
    }
}
