// <copyright file="ConnectionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public class ConnectionService : IConnectionService
{
    private readonly object _setupLock = new();

    private readonly ISettingsService _settingsService;

    private readonly HubConnection? _injectedConnection;
    private HubConnection? _hubConnection;
    private string _lastServerAddress = string.Empty;

    public ConnectionService(ISettingsService settingsService, HubConnection? hubConnection = null)
    {
        _settingsService = settingsService;
        settingsService.SettingsChanged += async (settings) => { await SetUpConnection(settings); };

        _injectedConnection = hubConnection;
    }

    public event EventHandler<HubConnection>? ConnectionChanged;

    public async Task<HubConnection> GetHubConnection()
    {
        if (_hubConnection != null)
        {
            return _hubConnection;
        }

        var settings = await _settingsService.GetSettings();

        await SetUpConnection(settings);

        return _hubConnection ??
               throw new NullReferenceException($"Connection is null despite calling {nameof(SetUpConnection)}");
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
                    // Not supported on a browser, browsers handle this themselves.
                }
            }

            return handler;
        };

        try
        {
            opts.WebSocketConfiguration = socket =>
            {
                socket.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            };
        }
        catch (PlatformNotSupportedException)
        {
            // Not supported on a browser, browsers handle this themselves.
        }
    }

    private async Task SetUpConnection(ImmutableSettings settings)
    {
        lock (_setupLock)
        {
            if (settings.ServerAddress == _lastServerAddress)
            {
                return;
            }

            _lastServerAddress = settings.ServerAddress;

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
        }

        try
        {
            await _hubConnection.StartAsync();
            ConnectionChanged?.Invoke(this, _hubConnection);
        }
        catch (Exception e)
        {
            // TODO #65
        }
    }
}