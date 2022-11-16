// <copyright file="ConnectionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public class ConnectionService : IConnectionService
{
    private readonly SemaphoreSlim _setupSemaphore = new(1);

    private readonly ISettingsService _settingsService;

    private HubConnection? _hubConnection;

    private bool _isConnectionGood;
    private string _currentServerAddress = string.Empty;

    public ConnectionService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        settingsService.SettingsChanged += HandleSettingsChanged;
    }

    public ConnectionService(ISettingsService settingsService, HubConnection hubConnection)
    {
        _settingsService = settingsService;
        _hubConnection = hubConnection;

        // Constructor is for injecting HubConnection in a test scenario.
        // IMHO .Wait()ing is the cleanest way to do this without messing with everything else.
        _hubConnection.StartAsync().Wait();
        _isConnectionGood = true;
    }

    public event EventHandler<HubConnection?>? ConnectionChanged;

    public async Task<HubConnection?> TryGetHubConnection()
    {
        await _setupSemaphore.WaitAsync();
        if (_isConnectionGood && _hubConnection is not null)
        {
            _setupSemaphore.Release();
            return _hubConnection;
        }

        var settings = await _settingsService.GetSettings();

        if (settings.ServerAddress == _currentServerAddress)
        {
            _setupSemaphore.Release();
            return null;
        }

        _hubConnection = CreateConnection(settings);
        _currentServerAddress = settings.ServerAddress;

        var connection = await StartConnection();
        _setupSemaphore.Release();
        return connection;
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

    private async void HandleSettingsChanged(ImmutableSettings settings)
    {
        if (settings.ServerAddress != _currentServerAddress)
        {
            await _setupSemaphore.WaitAsync();
            _hubConnection = CreateConnection(settings);
            _ = StartConnection();
            _setupSemaphore.Release();
        }
    }

    private async Task<HubConnection?> StartConnection()
    {
        if (_hubConnection is null)
        {
            throw new InvalidOperationException($"{nameof(StartConnection)} was called before a connection existed.");
        }

        try
        {
            await _hubConnection.StartAsync();
            _isConnectionGood = true;
            ConnectionChanged?.Invoke(this, _hubConnection);
            return _hubConnection;
        }
        catch (Exception)
        {
            _isConnectionGood = false;
            ConnectionChanged?.Invoke(this, null);
            return null;
        }
    }

    private HubConnection CreateConnection(ImmutableSettings settings)
    {
        var baseAddress = new Uri(settings.ServerAddress);
        var hubPath = new Uri(baseAddress, "/hub");

        return new HubConnectionBuilder().WithUrl(hubPath, (opts) =>
        {
            IgnoreInvalidCerts(opts);

            // The username is sent as access_token because it seemed like the easiest way to get SignalR to pass it along.
            opts.AccessTokenProvider = () => Task.FromResult((string?)settings.Username);
        })
            .AddJsonProtocol(options => options.PayloadSerializerOptions.AddContext<TauJsonContext>())
            .Build();
    }
}