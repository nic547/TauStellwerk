// <copyright file="Startup.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TauStellwerk.Commands;
using TauStellwerk.Database;
using TauStellwerk.Hub;
using TauStellwerk.Services;

namespace TauStellwerk;

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
        services.AddRazorPages();
        services.AddSignalR();

        services.AddDbContext<StwDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Database")));

        services.AddSingleton(new SessionService());
        services.AddSingleton(CommandSystemFactory.FromConfig(Configuration));
        services.AddSingleton(p =>
            new StatusService(p.GetRequiredService<CommandSystemBase>(), p.GetRequiredService<IHubContext<TauHub>>()));
        services.AddSingleton<IEngineService>(p =>
            new EngineService(p.GetRequiredService<CommandSystemBase>(), p.GetRequiredService<SessionService>()));

        services.AddScoped(p => new EngineRepo(p.GetRequiredService<StwDbContext>()));

        services.AddHostedService(p => p.GetRequiredService<SessionService>());
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
        }

        app.UseDefaultFiles();

        app.UseBlazorFrameworkFiles();

        EnsureContentDirectoriesExist();

        app.UseStaticFiles();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.GetFullPath(Configuration["generatedImageDirectory"])),
            RequestPath = "/images",
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapHub<TauHub>("/hub");
        });
    }

    private void EnsureContentDirectoriesExist()
    {
        Directory.CreateDirectory(Configuration["originalImageDirectory"]);
        Directory.CreateDirectory(Configuration["generatedImageDirectory"]);
    }
}