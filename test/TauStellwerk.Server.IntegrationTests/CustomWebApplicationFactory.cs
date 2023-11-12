// <copyright file="CustomWebApplicationFactory.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

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
