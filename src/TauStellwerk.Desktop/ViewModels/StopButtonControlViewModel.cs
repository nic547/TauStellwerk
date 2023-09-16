// <copyright file="StopButtonControlViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Resources;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class StopButtonControlViewModel : ViewModelBase
{
    private readonly StatusService _statusService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _lastAction = string.Empty;

    [ObservableProperty]
    private string _lastActionUsername = string.Empty;

    [ObservableProperty]
    private string _currentState = string.Empty;

    [ObservableProperty]
    private bool _shouldBeEnabled;

    private SystemStatus? _lastSystemStatus;

    public StopButtonControlViewModel()
    {
        _settingsService = Locator.Current.GetService<ISettingsService>() ?? throw new Exception("Failed to locate SettingsService");

        _statusService = Locator.Current.GetService<StatusService>() ?? throw new Exception("Failed to locate StatusService");
        _statusService.StatusChanged += HandleStatusChange;

        // Initial variables are populated initallialy with the last known status
        HandleStatusChange(null, _statusService.LastKnownStatus);
    }

    private void HandleStatusChange(object? sender, SystemStatus? e)
    {
        _lastSystemStatus = e;

        switch (e?.State)
        {
            case State.On:
                ShouldBeEnabled = true;
                CurrentState = Resources.StatusRunning;
                LastAction = Resources.StatusRunningSubtitle;
                LastActionUsername = e.LastActionUsername;
                break;
            case State.Off:
                ShouldBeEnabled = false;
                CurrentState = Resources.StatusStoppedLocked;
                LastAction = Resources.StatusStoppedSubtitle;
                LastActionUsername = e.LastActionUsername;
                HandleLockedStatus();
                break;
            case null:
                ShouldBeEnabled = false;
                CurrentState = Resources.StatusUnknown;
                LastAction = Resources.StatusUnknownSubtitle;
                LastActionUsername = string.Empty;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(StopButtonState.State), "Unknown value for State enum");
        }
    }

    private async void HandleLockedStatus()
    {
        await Task.Delay(3000);

        if (_lastSystemStatus?.State == State.Off)
        {
            ShouldBeEnabled = true;
            CurrentState = Resources.StatusStopped;
        }
    }

    [RelayCommand]
    private async Task StopButton()
    {
        var isCurrentlyRunning = _statusService.LastKnownStatus?.State;
        var username = (await _settingsService.GetSettings()).Username;
        var status = new SystemStatus()
        {
            State = isCurrentlyRunning == State.On ? State.Off : State.On,
            LastActionUsername = username,
        };

        await _statusService.SetStatus(status);
    }
}