// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using JetBrains.Annotations;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.ViewModels.Engine;
using TauStellwerk.Desktop.Views;
using TauStellwerk.Desktop.Views.Engine;

namespace TauStellwerk.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly StatusService _statusService;

        public MainWindowViewModel(StatusService? statusService = null, SettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? Locator.Current.GetService<SettingsService>() ?? throw new InvalidOperationException();
            _statusService = statusService ?? Locator.Current.GetService<StatusService>() ?? throw new InvalidOperationException();
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
            var vm = new EngineSelectionViewModel();
            var engineWindow = new EngineSelectionWindow
            {
                DataContext = vm,
                ViewModel = vm,
            };
            engineWindow.Show();
        }

        [UsedImplicitly]
        private void OpenSettings()
        {
            new SettingsWindow().Show();
        }
    }
}