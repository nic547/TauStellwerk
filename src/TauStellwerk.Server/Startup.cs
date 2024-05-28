// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using TauStellwerk.Base;
using TauStellwerk.Base.Model;
using TauStellwerk.CommandStations;
using TauStellwerk.Data;
using TauStellwerk.Data.Dao;
using TauStellwerk.Data.ImageService;
using TauStellwerk.Server.Hub;
using TauStellwerk.Server.Services;
using TauStellwerk.Server.Services.EngineControlService;
using TauStellwerk.Server.Services.TransferService;

namespace TauStellwerk.Server;

/// <summary>
/// Startup class of the WebAPI.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Startup"/> class.
/// </remarks>
/// <param name="configuration"><see cref="Configuration"/>.</param>
public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    private TauStellwerkOptions Options { get; } = configuration.Get<TauStellwerkOptions>() ?? throw new FormatException("Could not parse configuration");

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services">IDK.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.Configure<TauStellwerkOptions>(Configuration);

        services.AddRazorPages();
        services.AddSignalR()
            .AddJsonProtocol(options => options.PayloadSerializerOptions.TypeInfoResolver = TauJsonContext.Default);

        services.AddDbContextPool<StwDbContext>(options =>
            options.UseSqlite(Options.Database.ConnectionString));
        services.AddDbContextFactory<StwDbContext>(options =>
            options.UseSqlite(Options.Database.ConnectionString));

        services.AddSingleton(p => new SessionService(p.GetRequiredService<ILogger<SessionService>>()));
        services.AddSingleton(p => CommandStationFactory.FromConfig(Configuration, p.GetRequiredService<ILogger<CommandStationBase>>()));
        services.AddSingleton(p => new StatusControlService(
                p.GetRequiredService<CommandStationBase>(),
                p.GetRequiredService<IHubContext<TauHub>>(),
                p.GetRequiredService<ILogger<StatusControlService>>(),
                p.GetRequiredService<SessionService>(),
                p.GetRequiredService<IOptions<TauStellwerkOptions>>()));

        services.AddSingleton<IEngineControlService>(p => new EngineControlService(
            p.GetRequiredService<CommandStationBase>(),
            p.GetRequiredService<SessionService>(),
            p.GetRequiredService<ILogger<EngineControlService>>(),
            p.GetRequiredService<IOptions<TauStellwerkOptions>>()));

        services.AddSingleton<TurnoutControlService>();

        services.AddScoped(p => new EngineDao(p.GetRequiredService<StwDbContext>(), p.GetRequiredService<ILogger<EngineDao>>()));
        services.AddScoped<ITurnoutDao>(p => new TurnoutDao(p.GetRequiredService<StwDbContext>()));

        services.AddScoped(p => new ImageService(
            p.GetRequiredService<StwDbContext>(),
            p.GetRequiredService<ILogger<ImageService>>(),
            Options.OriginalImageDirectory,
            Options.GeneratedImageDirectory));

        services.AddScoped<ITransferService>(p => new TransferService(
            p.GetRequiredService<IDbContextFactory<StwDbContext>>(),
            p.GetRequiredService<IHubContext<TauHub>>(),
            p.GetRequiredService<ILogger<TransferService>>(),
            Options));
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
            FileProvider = new PhysicalFileProvider(Path.GetFullPath(Options.GeneratedImageDirectory)),
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

    private static FileExtensionContentTypeProvider GetContentTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();

        provider.Mappings.Add(".avif", "image/avif");

        return provider;
    }

    private static void Shutdown(IServiceProvider sp)
    {
        var commandSystem = sp.GetRequiredService<CommandStationBase>();
        commandSystem.HandleSystemStatus(State.Off).Wait();
    }

    private void EnsureContentDirectoriesExist()
    {
        Directory.CreateDirectory(Options.OriginalImageDirectory);
        Directory.CreateDirectory(Options.GeneratedImageDirectory);
        Directory.CreateDirectory(Options.DataTransferDirectory);
    }
}
