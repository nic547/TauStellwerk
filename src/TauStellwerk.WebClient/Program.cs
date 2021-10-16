// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using TauStellwerk.Client.Services;

namespace TauStellwerk.WebClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped<ISettingsService>(sp => new BlazorSettingsService(builder.HostEnvironment.BaseAddress, sp.GetRequiredService<IJSRuntime>()));
        builder.Services.AddScoped(provider => new HttpClientService(provider.GetRequiredService<ISettingsService>()));
        builder.Services.AddScoped(provider => new StatusService(provider.GetRequiredService<HttpClientService>()));
        builder.Services.AddScoped(provider => new EngineService(provider.GetRequiredService<HttpClientService>()));

        builder.Services.AddScoped(_ => new ModalManager());
        builder.Services.AddScoped(_ => new AppState());

        await builder.Build().RunAsync();
    }
}