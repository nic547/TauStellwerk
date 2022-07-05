// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly StatusService _statusService;
    private readonly IViewService _viewService;

    public MainWindowViewModel(StatusService? statusService = null, ISettingsService? settingsService = null, IViewService? viewService = null)
    {
        _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();
        _statusService = statusService ?? Locator.Current.GetService<StatusService>() ?? throw new InvalidOperationException();
        _viewService = viewService ?? Locator.Current.GetService<IViewService>() ?? throw new InvalidOperationException();

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

    [ICommand]
    private async Task StopButton()
    {
        var isCurrentlyRunning = _statusService.LastKnownStatus?.State;
        var username = (await _settingsService.GetSettings()).Username;
        var status = new SystemStatus()
        {
            State = isCurrentlyRunning == State.Off ? State.Off : State.On,
            LastActionUsername = username,
        };

        await _statusService.SetStatus(status);
    }

    [ICommand]
    private void OpenEngineList()
    {
        _viewService.ShowEngineSelectionView(this);
    }

    [ICommand]
    private void OpenSettings()
    {
        _viewService.ShowSettingsView(this);
    }
}