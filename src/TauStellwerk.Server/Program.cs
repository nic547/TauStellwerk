// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using TauStellwerk.Server.CommandStations;
using TauStellwerk.Server.Data;
using TauStellwerk.Server.Services;

namespace TauStellwerk.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .UseSystemd() // This shouldn't do anything on non-systemd systems.
            .ConfigureLogging(config =>
            {
                config.AddConsole(options => options.FormatterName = nameof(CustomLogFormatter))
                    .AddConsoleFormatter<CustomLogFormatter, SimpleConsoleFormatterOptions>();
            })
            .Build()
            .PrintVersionInformation()
            .MigrateDatabase()
            .LoadEngines()
            .SetupImages()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    public static IHost PrintVersionInformation(this IHost host)
    {
        var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();

        using var scope = serviceScopeFactory.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Startup>>(); // Wrong class - but Program doesn't seem to work.

        logger.LogInformation("--- TauStellwerk {AssemblyInformationalVersion} (.NET {Version}) ---", ThisAssembly.AssemblyInformationalVersion, Environment.Version);

        return host;
    }

    public static IHost MigrateDatabase(this IHost host)
    {
        var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();

        using var scope = serviceScopeFactory.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Startup>>(); // Wrong class - but Program doesn't seem to work.
        var dbContext = services.GetRequiredService<StwDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Applying database migrations.");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied.");
        }
        else
        {
            logger.LogInformation("No database migrations necessary.");
        }

        return host;
    }

    public static IHost LoadEngines(this IHost host)
    {
        var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();
        var scope = serviceScopeFactory.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<StwDbContext>();
        var commandSystem = services.GetRequiredService<CommandStationBase>();

        _ = commandSystem.LoadEnginesFromSystem(dbContext);

        return host;
    }

    public static IHost SetupImages(this IHost host)
    {
        var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();
        var scope = serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var options = services.GetRequiredService<IOptions<TauStellwerkOptions>>().Value;
        var logger = services.GetRequiredService<ILogger<ImageService>>();

        var system = new ImageService(
            services.GetRequiredService<StwDbContext>(),
            logger,
            options.OriginalImageDirectory,
            options.GeneratedImageDirectory);
        _ = Task.Run(system.RunImageSetup);

        return host;
    }
}