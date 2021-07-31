// <copyright file="SplatFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Splat;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop
{
    /// <summary>
    /// Extension factory methods for client services that work with Splat DI.
    /// </summary>
    public static class SplatFactory
    {
        public static ClientEngineService CreateClientEngineService()
        {
            var httpService = Locator.Current.GetService<ClientHttpService>() ?? throw new InvalidOperationException();
            return new ClientEngineService(httpService);
        }

        public static ClientStatusService CreateClientStatusService()
        {
            var httpService = Locator.Current.GetService<ClientHttpService>() ?? throw new InvalidOperationException();
            return new ClientStatusService(httpService);
        }

        public static ClientHttpService CreateClientHttpService()
        {
            var settingService = Locator.Current.GetService<ClientSettingsService>() ?? throw new InvalidOperationException();
            return new ClientHttpService(settingService);
        }
    }
}
