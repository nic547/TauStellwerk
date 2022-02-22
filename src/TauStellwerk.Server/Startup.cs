// <copyright file="Startup.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Database;
using TauStellwerk.Hub;
using TauStellwerk.Services;
using TauStellwerk.Services.EngineService;

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
        services.AddLogging();
        services.AddRazorPages();
        services.AddSignalR()
            .AddJsonProtocol(options => options.PayloadSerializerOptions.AddContext<TauJsonContext>());

        services.AddDbContext<StwDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Database")));

        services.AddSingleton(p => new SessionService(p.GetRequiredService<ILogger<SessionService>>()));
        services.AddSingleton(p => CommandSystemFactory.FromConfig(Configuration, p.GetRequiredService<ILogger<CommandSystemBase>>()));
        services.AddSingleton(p =>
            new StatusService(p.GetRequiredService<CommandSystemBase>(), p.GetRequiredService<IHubContext<TauHub>>(), p.GetRequiredService<ILogger<StatusService>>()));
        services.AddSingleton<IEngineService>(p =>
            new EngineService(p.GetRequiredService<CommandSystemBase>(), p.GetRequiredService<SessionService>(), p.GetRequiredService<ILogger<EngineService>>()));

        services.AddScoped(p => new EngineRepo(p.GetRequiredService<StwDbContext>(), p.GetRequiredService<ILogger<EngineRepo>>()));
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
            ContentTypeProvider = GetContentTypeProvider(),
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapHub<TauHub>("/hub");
        });

        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStopping.Register(() => Shutdown(app.ApplicationServices));
    }

    private static IContentTypeProvider GetContentTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();

        provider.Mappings.Add(".avif", "image/avif");

        return provider;
    }

    private static void Shutdown(IServiceProvider sp)
    {
        var commandSystem = sp.GetRequiredService<CommandSystemBase>();
        commandSystem.HandleSystemStatus(State.Off).Wait();
    }

    private void EnsureContentDirectoriesExist()
    {
        Directory.CreateDirectory(Configuration["originalImageDirectory"]);
        Directory.CreateDirectory(Configuration["generatedImageDirectory"]);
    }
}