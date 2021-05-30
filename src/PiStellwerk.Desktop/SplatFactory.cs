using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiStellwerk.Client.Services;
using Splat;

namespace PiStellwerk.Desktop
{
    /// <summary>
    /// Extension factory methods for client services that work with Splat DI.
    /// </summary>
    public static class SplatFactory
    {
        public static ClientEngineService CreateClientEngineService()
        {
            var httpService = Locator.Current.GetService<ClientHttpService>() ?? throw new InvalidOperationException();
            return new ClientEngineService(httpService);
        }

        public static ClientStatusService CreateClientStatusService()
        {
            var httpService = Locator.Current.GetService<ClientHttpService>() ?? throw new InvalidOperationException();
            return new ClientStatusService(httpService);
        }

        public static ClientHttpService CreateClientHttpService()
        {
            var settingService = Locator.Current.GetService<ClientSettingsService>() ?? throw new InvalidOperationException();
            return new ClientHttpService(settingService);
        }
    }
}
