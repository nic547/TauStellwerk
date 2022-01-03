// <copyright file="ConnectionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace TauStellwerk.Client.Services;

public class ConnectionService : IConnectionService
{
    private readonly ISettingsService _settingsService;

    private readonly Timer _sessionTimer;

    private readonly HubConnection? _injectedConnection;

    private HubConnection? _hubConnection;

    public ConnectionService(ISettingsService settingsService, HubConnection? hubConnection = null)
    {
        _settingsService = settingsService;

        _injectedConnection = hubConnection;

        _sessionTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
        _sessionTimer.Elapsed += KeepSessionAlive;
        _sessionTimer.AutoReset = true;
    }

    public async Task<HubConnection> GetHubConnection()
    {
        if (_hubConnection != null)
        {
            return _hubConnection;
        }

        if (_injectedConnection != null)
        {
            _hubConnection = _injectedConnection;
        }
        else
        {
            var baseAddress = new Uri((await _settingsService.GetSettings()).ServerAddress);
            var hubPath = new Uri(baseAddress, "/hub");
            Console.WriteLine(hubPath);

            _hubConnection = new HubConnectionBuilder().WithUrl(hubPath, IgnoreInvalidCerts).Build();
        }

        var username = (await _settingsService.GetSettings()).Username;

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync("RegisterUser", username);

        _sessionTimer.Start();

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

    private void KeepSessionAlive(object? sender, ElapsedEventArgs e)
    {
        if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
        {
            return;
        }

        _hubConnection.InvokeAsync("SendHeartbeat");
    }
}