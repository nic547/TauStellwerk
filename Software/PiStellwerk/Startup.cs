// <copyright file="Startup.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Services;
using PiStellwerk.Util;

namespace PiStellwerk
{
    /// <summary>
    /// Startup class of the WebAPI.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration"><see cref="Configuration"/>.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            using var client = new StwDbContext();
            if (client.Database.GetPendingMigrations().Any())
            {
                ConsoleService.PrintHighlightedMessage("Applying database migrations.");
                client.Database.Migrate();
                ConsoleService.PrintHighlightedMessage("Database migrations applied.");
            }
            else
            {
                ConsoleService.PrintMessage("No database migrations necessary.");
            }

            // UNCOMMENT TO ADD TEST DATA.
            // client.Engines.AddRange(TestDataService.GetEngines());
            // client.SaveChanges();
        }

        /// <summary>
        /// Gets some configuration.
        /// TODO: Find out what exactly this configuration is, sounds pretty useful.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IDK.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddEntityFrameworkSqlite().AddDbContext<StwDbContext>();

            services.AddHostedService<SessionService>();

            var commandSystem = CommandSystemFactory.FromConfig(Configuration);
            _ = commandSystem.LoadEnginesFromSystem(new StwDbContext());
            services.AddSingleton(commandSystem);

            services.AddSingleton(new StatusService(commandSystem));
            services.AddSingleton(new EngineService(commandSystem));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IDK.</param>
        /// <param name="env">IDK either.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new CompositeFileProvider(
                    new PhysicalFileProvider(
                        Path.Combine(env.ContentRootPath, "userImages")),
                    new PhysicalFileProvider(
                        Path.Combine(env.ContentRootPath, "wwwroot"))),
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
