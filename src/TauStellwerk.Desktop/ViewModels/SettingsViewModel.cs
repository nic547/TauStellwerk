// <copyright file="SettingsViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private MutableSettings? _settings;

        public SettingsViewModel(SettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<SettingsService>() ?? throw new InvalidOperationException();
            _ = LoadSettings();
        }

        public MutableSettings? Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }

        public async Task LoadSettings()
        {
            Settings = await _settingsService.GetMutableSettingsCopy();
        }

        public async Task SaveSettings()
        {
            if (_settings != null)
            {
                await _settingsService.SetSettings(_settings);
            }
        }
    }
}
