// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.ViewModels.Engine;
using TauStellwerk.Desktop.Views;
using TauStellwerk.Desktop.Views.Engine;

namespace TauStellwerk.Desktop.ViewModels;

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

        StopButtonCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleStopButton);
        OpenEngineListCommand = ReactiveCommand.Create<Unit, Unit>(HandleOpenEngineList);
        OpenSettingsCommand = ReactiveCommand.Create<Unit, Unit>(HandleOpenSettings);
    }

    public StopButtonState StopButtonState { get; } = new();

    public ReactiveCommand<Unit, Unit> StopButtonCommand { get; }

    public ReactiveCommand<Unit, Unit> OpenEngineListCommand { get; }

    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

    private async Task<Unit> HandleStopButton(Unit param)
    {
        var isCurrentlyRunning = _statusService.LastKnownStatus?.IsRunning;
        var username = (await _settingsService.GetSettings()).Username;
        var status = new Status()
        {
            IsRunning = !isCurrentlyRunning ?? true,
            LastActionUsername = username,
        };

        await _statusService.SetStatus(status);
        return Unit.Default;
    }

    private Unit HandleOpenEngineList(Unit param)
    {
        var vm = new EngineSelectionViewModel();
        var engineWindow = new EngineSelectionWindow(vm);
        engineWindow.Show();

        return Unit.Default;
    }

    private Unit HandleOpenSettings(Unit param)
    {
        new SettingsWindow().Show();
        return Unit.Default;
    }
}