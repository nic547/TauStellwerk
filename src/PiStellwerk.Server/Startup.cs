// <copyright file="Startup.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PiStellwerk.Commands;
using PiStellwerk.Database;
using PiStellwerk.Services;

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
        }

        /// <summary>
        /// Gets the <see cref="IConfiguration"/> this application was started with.
        /// </summary>
        private IConfiguration Configuration { get; }

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

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddDbContext<StwDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("Database")));

            services.AddSingleton(new SessionService());
            services.AddSingleton(CommandSystemFactory.FromConfig(Configuration));
            services.AddSingleton(p => new StatusService(p.GetRequiredService<CommandSystemBase>()));
            services.AddSingleton<IEngineService>(p => new EngineService(p.GetRequiredService<CommandSystemBase>(), p.GetRequiredService<SessionService>()));

            services.AddHostedService(p => p.GetRequiredService<SessionService>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PiStellwerk API", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "PiStellwerk.xml");
                c.IncludeXmlComments(filePath);
            });
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
                app.UseWebAssemblyDebugging();

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PiStellwerk API V1");
                });
            }

            app.UseDefaultFiles();

            app.UseBlazorFrameworkFiles();

            EnsureContentDirectoriesExist(env);

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, Configuration["generatedImageDirectory"])),
                RequestPath = "/images",
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        private void EnsureContentDirectoriesExist(IHostEnvironment env)
        {
            Directory.CreateDirectory(Configuration["originalImageDirectory"]);
            Directory.CreateDirectory(Configuration["generatedImageDirectory"]);
        }
    }
}
