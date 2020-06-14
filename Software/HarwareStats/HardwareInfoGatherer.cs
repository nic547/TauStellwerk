// <copyright file="HardwareInfoGatherer.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace HardwareInfo
{
    /// <summary>
    /// Provides Functionality related to things like memory usage, cpu usage, cpu temperature etc.
    /// Results depend heavily on OS/device etc.
    /// </summary>
    public class HardwareInfoGatherer
    {
        private static readonly List<IInfoProvider> _availableProviders = new List<IInfoProvider>();
        private static bool _hasBeenInitialized = false;

        /// <summary>
        /// Gets a list of all Hardware related statistics available on the system.
        /// </summary>
        /// <returns>List of <see cref="InfoRecord"/>.</returns>
        public static List<InfoRecord> Get()
        {
            if (!_hasBeenInitialized)
            {
                Init();
            }

            List<InfoRecord> results = new List<InfoRecord>();

            foreach (var provider in _availableProviders)
            {
                results.AddRange(provider.GetInfoRecords());
            }

            return results;
        }

        private static void Init()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IInfoProvider).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var type in types)
            {
                var provider = (IInfoProvider)Activator.CreateInstance(type);

                if (provider.CheckAvailability())
                {
                    _availableProviders.Add(provider);
                }
            }

            _hasBeenInitialized = true;
        }
    }
}
