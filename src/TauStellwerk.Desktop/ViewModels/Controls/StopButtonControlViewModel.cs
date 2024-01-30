// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base.Model;
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

    [ObservableProperty]
    private SystemStatus? _lastSystemStatus;

    [ObservableProperty]
    private string buttonClass = "Unknown";

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
        LastSystemStatus = e;
        switch (e?.State)
        {
            case State.On:
                ShouldBeEnabled = true;
                CurrentState = Resources.StatusRunning;
                LastAction = Resources.StatusRunningSubtitle;
                LastActionUsername = e.LastActionUsername;
                ButtonClass = "Running";
                break;
            case State.Off:
                ShouldBeEnabled = false;
                CurrentState = Resources.StatusStoppedLocked;
                LastAction = Resources.StatusStoppedSubtitle;
                LastActionUsername = e.LastActionUsername;
                ButtonClass = "Locked";
                HandleLockedStatus();
                break;
            case null:
                ShouldBeEnabled = false;
                CurrentState = Resources.StatusUnknown;
                LastAction = Resources.StatusUnknownSubtitle;
                LastActionUsername = string.Empty;
                ButtonClass = "Unknown";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(StopButtonState.State), "Unknown value for State enum");
        }
    }

    private async void HandleLockedStatus()
    {
        await Task.Delay(3000);

        if (LastSystemStatus?.State == State.Off)
        {
            ShouldBeEnabled = true;
            CurrentState = Resources.StatusStopped;
            ButtonClass = "Stopped";
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
