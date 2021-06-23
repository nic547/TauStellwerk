// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PiStellwerk.Client.Services;

namespace PiStellwerk.WebClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IClientSettingsService>(sp => new BlazorSettingsService(builder.HostEnvironment.BaseAddress, sp.GetRequiredService<ILocalStorageService>()));
            builder.Services.AddScoped(provider => new ClientHttpService(provider.GetRequiredService<IClientSettingsService>()));
            builder.Services.AddScoped(provider => new ClientStatusService(provider.GetRequiredService<ClientHttpService>()));
            builder.Services.AddScoped(provider => new ClientEngineService(provider.GetRequiredService<ClientHttpService>()));

            builder.Services.AddScoped(_ => new ModalManager());
            builder.Services.AddScoped(_ => new AppState());

            await builder.Build().RunAsync();
        }
    }
}
