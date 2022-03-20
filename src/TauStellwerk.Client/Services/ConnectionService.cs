// <copyright file="ConnectionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace TauStellwerk.Client.Services;

public class ConnectionService : IConnectionService
{
    private readonly ISettingsService _settingsService;

    private readonly HubConnection? _injectedConnection;

    private HubConnection? _hubConnection;

    public ConnectionService(ISettingsService settingsService, HubConnection? hubConnection = null)
    {
        _settingsService = settingsService;

        _injectedConnection = hubConnection;
    }

    public async Task<HubConnection> GetHubConnection()
    {
        if (_hubConnection != null)
        {
            return _hubConnection;
        }

        var settings = await _settingsService.GetSettings();

        if (_injectedConnection != null)
        {
            _hubConnection = _injectedConnection;
        }
        else
        {
            var baseAddress = new Uri(settings.ServerAddress);
            var hubPath = new Uri(baseAddress, "/hub");

            _hubConnection = new HubConnectionBuilder().WithUrl(hubPath, (opts) =>
            {
                IgnoreInvalidCerts(opts);

                // The username is sent as access_token because it seemed like the easiest way to get SignalR to pass it along.
                opts.AccessTokenProvider = () => Task.FromResult((string?)settings.Username);
            }).Build();
        }

        await _hubConnection.StartAsync();

        return _hubConnection;
    }

    private static void IgnoreInvalidCerts(HttpConnectionOptions opts)
    {
        opts.HttpMessageHandlerFactory = (handler) =>
        {
            if (handler is HttpClientHandler clientHandler)
            {
                try
                {
                    clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                }
                catch (NotSupportedException)
                {
                    // Not all platforms support the certificate handling, for example Blazor WebAssembly doesn't.
                    // Luckily the invalid certificates are handled by the browser in that case.
                }
            }

            return handler;
        };
    }
}