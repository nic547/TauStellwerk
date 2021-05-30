// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using JetBrains.Annotations;
using PiStellwerk.Client.Model;
using PiStellwerk.Client.Services;
using PiStellwerk.Data;
using PiStellwerk.Desktop.Views;
using Splat;

namespace PiStellwerk.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ClientSettingsService _settingsService;
        private ClientStatusService _statusService;

        public MainWindowViewModel(ClientStatusService? statusService = null, ClientSettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ClientSettingsService>() ?? throw new InvalidOperationException();
            _statusService = statusService ?? Locator.Current.GetService<ClientStatusService>() ?? throw new InvalidOperationException();
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