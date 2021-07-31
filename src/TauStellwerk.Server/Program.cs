// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TauStellwerk.Commands;
using TauStellwerk.Database;
using TauStellwerk.Images;
using TauStellwerk.Util;

namespace TauStellwerk
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
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
                })
            .ConfigureHostConfiguration((config) =>
            config.AddInMemoryCollection(DefaultConfiguration.Values));

        public static IHost MigrateDatabase(this IHost host)
        {
            var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();

            using var scope = serviceScopeFactory.CreateScope();

            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<StwDbContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                ConsoleService.PrintHighlightedMessage("Applying database migrations.");
                dbContext.Database.Migrate();
                ConsoleService.PrintHighlightedMessage("Database migrations applied.");
            }
            else
            {
                ConsoleService.PrintMessage("No database migrations necessary.");
            }

            return host;
        }

        public static IHost LoadEngines(this IHost host)
        {
            var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();
            var scope = serviceScopeFactory.CreateScope();

            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<StwDbContext>();
            var commandSystem = services.GetRequiredService<CommandSystemBase>();

            _ = commandSystem.LoadEnginesFromSystem(dbContext);

            return host;
        }

        public static IHost SetupImages(this IHost host)
        {
            var serviceScopeFactory = (IServiceScopeFactory?)host.Services.GetService(typeof(IServiceScopeFactory)) ?? throw new ApplicationException();
            var scope = serviceScopeFactory.CreateScope();
            var services = scope.ServiceProvider;

            var config = services.GetRequiredService<IConfiguration>();
            var system = new ImageSystem(
                services.GetRequiredService<StwDbContext>(),
                config["originalImageDirectory"],
                config["generatedImageDirectory"]);
            _ = Task.Run(system.RunImageSetup);

            return host;
        }
    }
}
