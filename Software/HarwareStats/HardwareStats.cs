using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HardwareInfo
{
    /// <summary>
    /// Provides Functionality related to things like memory usage, cpu usage, cpu temperature etc.
    /// Results depend heavily on OS/device etc. !
    /// </summary>
    public class HardwareInfoGatherer
    {
        private static bool _hasBeenInitialized = false;
        private static List<IStatProvider> _availableProviders = new List<IStatProvider>();
        private static void Init()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IStatProvider).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var type in types)
            {
                var provider = (IStatProvider)Activator.CreateInstance(type);

                if (provider.CheckAvailability())
                {
                    _availableProviders.Add(provider);
                }
            }

            _hasBeenInitialized = true;
        }

        public static List<Stat> Get()
        {
            if (!_hasBeenInitialized)
            {
                Init();
            }

            List<Stat> results = new List<Stat>();

            foreach (var provider in _availableProviders) {
                results.AddRange(provider.GetStats());
            }
            return results;
        }
    }
}
