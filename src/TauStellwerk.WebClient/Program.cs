// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using TauStellwerk.Client.Services;
using TauStellwerk.Client.Services.Connections;

namespace TauStellwerk.WebClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped<ISettingsService>(sp => new BlazorSettingsService(builder.HostEnvironment.BaseAddress, sp.GetRequiredService<IJSRuntime>()));
        builder.Services.AddScoped(provider => new ConnectionService(provider.GetRequiredService<ISettingsService>()));
        builder.Services.AddScoped(provider => new StatusService(provider.GetRequiredService<ConnectionService>()));
        builder.Services.AddScoped(provider => new EngineService(provider.GetRequiredService<ConnectionService>()));

        builder.Services.AddScoped(_ => new ModalManager());
        builder.Services.AddScoped(provider => new AppState(provider.GetRequiredService<ModalManager>()));

        await builder.Build().RunAsync();
    }
}
