// <copyright file="BlazorSettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using PiStellwerk.Client.Model;
using PiStellwerk.Client.Services;

namespace PiStellwerk.WebClient
{
    public class BlazorSettingsService : IClientSettingsService
    {
        private const string _settingsKey = "PiStellwerk_Settings";

        private readonly string _baseAddress;
        private readonly ILocalStorageService _storageService;

        private bool _hasLoadBeenAttempted;

        private MutableSettings _settings;
        private ImmutableSettings _immutableSettings;

        public BlazorSettingsService(string baseAddress, ILocalStorageService storageService)
        {
            _baseAddress = baseAddress;
            _storageService = storageService;
            _settings = new MutableSettings
            {
                ServerAddress = baseAddress,
            };
            _immutableSettings = _settings.GetImmutableCopy();
        }

        public event IClientSettingsService.SettingsChange? SettingsChanged;

        public async Task<ImmutableSettings> GetSettings()
        {
            await EnsureSettingsWereLoaded();
            return _immutableSettings;
        }

        public async Task<MutableSettings> GetMutableSettingsCopy()
        {
            await EnsureSettingsWereLoaded();
            return _settings.GetMutableCopy();
        }

        public async Task SetSettings(MutableSettings mutableSettings)
        {
            mutableSettings.ServerAddress = _baseAddress;
            _settings = mutableSettings;

            _immutableSettings = _settings.GetImmutableCopy();

            await _storageService.SetItemAsync(_settingsKey, mutableSettings);

            SettingsChanged?.Invoke(_immutableSettings);
        }

        /// <summary>
        /// Ensures that there was an attempt at loading the settings file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task EnsureSettingsWereLoaded()
        {
            if (_hasLoadBeenAttempted)
            {
                return;
            }

            try
            {
                var potentialSettings = await _storageService.GetItemAsync<MutableSettings>(_settingsKey);
                if (potentialSettings != null)
                {
                    potentialSettings.ServerAddress = _baseAddress;
                    _settings = potentialSettings;
                    _immutableSettings = _settings.GetImmutableCopy();
                    SettingsChanged?.Invoke(_immutableSettings);
                }
            }
            catch (Exception)
            {
                // ignore exception, file probably just doesn't exists yet.
            }

            _hasLoadBeenAttempted = true;
        }
    }
}