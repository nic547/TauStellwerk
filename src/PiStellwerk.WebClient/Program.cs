// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
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

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var settingsService = new BlazorSettingsService(builder.HostEnvironment.BaseAddress);
            var httpService = new ClientHttpService(settingsService);
            var statusService = new ClientStatusService(httpService);
            var engineService = new ClientEngineService(httpService);

            builder.Services.AddSingleton<IClientSettingsService>(_ => settingsService);
            builder.Services.AddSingleton(_ => httpService);
            builder.Services.AddSingleton(_ => statusService);
            builder.Services.AddSingleton(sp => engineService);

            builder.Services.AddSingleton(new ModalManager());
            builder.Services.AddSingleton(new AppState());

            await builder.Build().RunAsync();
        }
    }
}
