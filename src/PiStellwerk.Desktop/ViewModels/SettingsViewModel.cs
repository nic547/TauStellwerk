// <copyright file="SettingsViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using PiStellwerk.Desktop.Model;
using PiStellwerk.Desktop.Services;
using ReactiveUI;
using Splat;

namespace PiStellwerk.Desktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private Settings? _settings;

        public SettingsViewModel(SettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<SettingsService>();
            LoadSettings();
        }

        public Settings? Settings
        {
            get => _settings;
            set => this.RaiseAndSetIfChanged(ref _settings, value);
        }

        public async void LoadSettings()
        {
            Settings = await _settingsService.GetSettings(true);
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
