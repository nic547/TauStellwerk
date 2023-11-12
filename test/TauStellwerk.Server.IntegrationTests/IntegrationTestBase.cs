// <copyright file="IntegrationTestBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests;

public class IntegrationTestBase
{
    private WebApplicationFactory<Startup> _factory = null!; // Created in setup

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Startup>();
    }

    protected IConnectionService CreateConnectionService()
    {
        var settingService = Substitute.For<ISettingsService>();
        settingService.GetSettings().Returns(new ImmutableSettings("TEST", _factory.Server.BaseAddress.ToString(), string.Empty, false, "en"));

        var hubConnection = new HubConnectionBuilder().WithUrl(
            _factory.Server.BaseAddress + "hub",
            options => options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler())
            .AddJsonProtocol(options => options.PayloadSerializerOptions.AddContext<TauJsonContext>())
            .Build();

        return new ConnectionService(settingService, hubConnection);
    }
}
