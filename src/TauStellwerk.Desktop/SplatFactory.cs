// <copyright file="SplatFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Splat;
using TauStellwerk.Client.Services;

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

    public static ConnectionService CreateClientHttpService()
    {
        var settingService = Locator.Current.GetService<SettingsService>() ?? throw new InvalidOperationException();
        return new ConnectionService(settingService);
    }
}