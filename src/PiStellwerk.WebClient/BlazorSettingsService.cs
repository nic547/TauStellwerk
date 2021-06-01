using System;
using System.Threading.Tasks;
using PiStellwerk.Client.Model;
using PiStellwerk.Client.Services;

namespace PiStellwerk.WebClient
{
    public class BlazorSettingsService : IClientSettingsService
    {
        private readonly string _baseAddress;

        public BlazorSettingsService(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public Task<Settings> GetSettings(bool forceReload = false)
        {
            var settings = new Settings()
            {
                ServerAddress = _baseAddress,
                Username = "BLAZOR TEST USER",
            };
            return Task.FromResult(settings);
        }

        public Task SetSettings(Settings settings)
        {
            throw new NotImplementedException();
        }
    }
}
