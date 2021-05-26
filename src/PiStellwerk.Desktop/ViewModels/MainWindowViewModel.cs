// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using JetBrains.Annotations;
using PiStellwerk.Data;
using PiStellwerk.Desktop.Model;
using PiStellwerk.Desktop.Services;
using PiStellwerk.Desktop.Views;
using Splat;

namespace PiStellwerk.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private StatusService _statusService;

        public MainWindowViewModel(StatusService? statusService = null, SettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<SettingsService>();
            _statusService = statusService ?? Locator.Current.GetService<StatusService>();
            _statusService.StatusChanged += (status) =>
            {
                StopButtonState.SetStatus(status);
            };
            if (_statusService.LastKnownStatus != null)
            {
                StopButtonState.SetStatus(_statusService.LastKnownStatus);
            }
        }

        public StopButtonState StopButtonState { get; } = new();

        [UsedImplicitly]
        private async void StopButtonCommand()
        {
            var isCurrentlyRunning = _statusService.LastKnownStatus?.IsRunning;
            var username = (await _settingsService.GetSettings()).Username;
            var status = new Status()
            {
                IsRunning = !isCurrentlyRunning ?? true,
                LastActionUsername = username,
            };

            await _statusService.SetStatus(status);
        }

        [UsedImplicitly]
        private void OpenEngineList()
        {
            var engineWindow = new EngineWindow();
            engineWindow.Show();
        }

        [UsedImplicitly]
        private void OpenSettings()
        {
            new SettingsWindow().Show();
        }
    }
}