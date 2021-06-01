using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
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

            Console.WriteLine("ADDING SERVICES");

            builder.Services.AddSingleton<IClientSettingsService>(_ => settingsService);
            builder.Services.AddSingleton(_ => httpService);
            builder.Services.AddSingleton(_ => statusService);

            Console.WriteLine("ADDED SERVICES");

            await builder.Build().RunAsync();
        }
    }
}
