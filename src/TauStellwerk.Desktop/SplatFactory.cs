// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Splat;
using TauStellwerk.Client.Services;
using TauStellwerk.Client.Services.Connections;
using TauStellwerk.Client.Services.DecoderProgramming;
using TauStellwerk.Client.Services.Turnouts;

namespace TauStellwerk.Desktop;

/// <summary>
/// Extension factory methods for client services that work with Splat DI.
/// </summary>
public static class SplatFactory
{
    public static EngineService CreateClientEngineService()
    {
        var httpService = Locator.Current.GetService<ConnectionService>() ?? throw new InvalidOperationException();
        return new EngineService(httpService);
    }

    public static StatusService CreateClientStatusService()
    {
        var httpService = Locator.Current.GetService<ConnectionService>() ?? throw new InvalidOperationException();
        return new StatusService(httpService);
    }

    public static DecoderProgrammingService CreateProgrammingService()
    {
        var connectionService = Locator.Current.GetService<ConnectionService>() ?? throw new Exception($"Failed to locate an instance of {nameof(IConnectionService)}");
        return new DecoderProgrammingService(connectionService);
    }

    public static ConnectionService CreateClientHttpService()
    {
        var settingService = Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();
        return new ConnectionService(settingService);
    }

    public static ITurnoutService CreateTurnoutService()
    {
        var connectionService = Locator.Current.GetService<ConnectionService>() ?? throw new InvalidOperationException();
        return new TurnoutService(connectionService);
    }
}
