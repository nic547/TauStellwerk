// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TauStellwerk.Data;

namespace TauStellwerk.Server.IntegrationTests;

public static class HostExtensions
{
    public static IHost CreateDatabase(this IHost host)
    {
        var serviceScopeFactory = host.Services.GetService(typeof(IServiceScopeFactory)) as IServiceScopeFactory ?? throw new ApplicationException();

        using var scope = serviceScopeFactory.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<StwDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return host;
    }
}
