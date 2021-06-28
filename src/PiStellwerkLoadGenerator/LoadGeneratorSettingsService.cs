// <copyright file="LoadGeneratorSettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using PiStellwerk.Client.Model;
using PiStellwerk.Client.Services;

namespace PiStellwerkLoadGenerator
{
    public class LoadGeneratorSettingsService : IClientSettingsService
    {
        private readonly ImmutableSettings _settings;

        public LoadGeneratorSettingsService(Options options, Random random)
        {
            _settings = new ImmutableSettings(
                $"Random User {random.Next(999_999)}",
                options.Uri.ToString(),
                "ThisIsNotATheme");
        }

        public event IClientSettingsService.SettingsChange SettingsChanged
        {
            add { }
            remove { }
        }

        public Task<ImmutableSettings> GetSettings()
        {
            return Task.FromResult(_settings);
        }

        public Task<MutableSettings> GetMutableSettingsCopy()
        {
            throw new NotImplementedException("Cannot edit settings for the load generator");
        }

        public Task SetSettings(MutableSettings mutableSettings)
        {
            throw new NotImplementedException("Cannot edit settings for the load generator");
        }
    }
}