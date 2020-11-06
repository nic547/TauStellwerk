// <copyright file="Startup.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PiStellwerk.Commands;
using PiStellwerk.Data;

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
            client.Database.EnsureDeleted();
            client.Database.EnsureCreated();
            if (client.Engines.Any())
            {
                return;
            }

            client.Engines.AddRange(TestDataService.GetEngines());
            client.SaveChanges();
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

            services.AddHostedService<BackgroundServices.UserService>();

            services.AddSingleton(CommandSystemFactory.FromConfig(Configuration));
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
