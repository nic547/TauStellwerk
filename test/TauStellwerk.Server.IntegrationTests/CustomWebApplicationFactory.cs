// <copyright file="CustomWebApplicationFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TauStellwerk.Database;
using TauStellwerk.Util;

namespace TauStellwerk.Server.IntegrationTests;

public class CustomWebApplicationFactory<T> : WebApplicationFactory<T>
    where T : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        return base.CreateHost(builder)
            .CreateDatabase()
            .LoadEngines()
            .SetupImages();
    }
}

public static class HostExtensions
{
    public static IHost CreateDatabase(this IHost host)
    {
        var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();

        using var scope = serviceScopeFactory.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<StwDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return host;
    }
}
