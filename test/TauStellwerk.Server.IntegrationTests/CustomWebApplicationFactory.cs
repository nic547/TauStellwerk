// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

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
